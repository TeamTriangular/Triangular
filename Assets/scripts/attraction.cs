using UnityEngine;
using System.Collections;

public class attraction : MonoBehaviour {
	public float rotationSpeed = 1.0f; // the speed at which the trinagle rotates to face what it is being attracted to
	public float attractionDistance = 1.5f; // the distance before a static triangle's gravity can effect a free floating one
	public float pullFactor = 1.0f; // strength of pull
	public float connectionDist = 0.1f;

	System.Collections.Generic.List<Transform> centerPoints;
	System.Collections.Generic.List<Transform> ignoredPoints;
	Vector3[] faceNormals = new Vector3[3];

	public TriangleGrid gridScript;

	/**
	 * Represents a PullForce will all variables needed for one. 
	 * @param Vector3 applyPos is the original point of the triangle this script is on
	 * @param Vector3 dstPos is the point that is closest to the original point, this is a corner or middle point
	 * depending on what the original point is. 
	 * @param Vector3 force is the closest point - the original point
	 */
	public struct PullForce
	{
		public Transform applyObj;
		public Transform dstObj;
		public Vector3 force;	
		public float dist;	
		
		public PullForce(Transform applyObj_, Transform dstObj_, Vector3 force_)
		{
			applyObj = applyObj_;
			dstObj = dstObj_;
			force = force_;
			dist = Vector3.Distance(applyObj.position, dstObj_.position);
		}
	};

	// Use this for initialization
	void Awake () {
		centerPoints = new System.Collections.Generic.List<Transform>();
		ignoredPoints = new System.Collections.Generic.List<Transform>();
		
		for(int i=0; i< transform.childCount; i++)
		{
			if(transform.GetChild(i).tag == "MiddleControlPoint")
			{
				centerPoints.Add(transform.GetChild(i));
			}
		}

		gridScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TriangleGrid>();

		faceNormals[0] = Quaternion.Euler( new Vector3(0, 0, 60)) * transform.up;
		faceNormals[1] = Quaternion.Euler( new Vector3(0, 0, 120)) * faceNormals[0];
		faceNormals[2] = Quaternion.Euler( new Vector3(0, 0, 120)) * faceNormals[1];

		updateIgnorePoints();
	}
	
	void updateIgnorePoints()
	{
		ignoredPoints.Clear();
		
		FixedJoint[] neighbours = GetComponents<FixedJoint>();
		System.Collections.Generic.List<Rigidbody> visitedNodes = new System.Collections.Generic.List<Rigidbody>();
		visitedNodes.Add (rigidbody); // add ourself so we dont visit ourself
		
		addNeighbours(neighbours, visitedNodes);
	}
	
	void addNeighbours(FixedJoint[] neighbours, System.Collections.Generic.List<Rigidbody> visitedNodes)
	{
		
		
		//look through all neighbours we are connected to 
		for(int i=0; i< neighbours.Length; i++)
		{
			bool alreadyExists = false;
			//check if we have already visited the triangle
			for(int k=0; k< visitedNodes.Count; k++)
			{
				if(visitedNodes[k] == neighbours[i].connectedBody )
				{
					alreadyExists = true;
					break;
				}
			}
			
			//if we have not visited the triange then add the control points for the triangle to the ignore list
			if(!alreadyExists)
			{
				visitedNodes.Add(neighbours[i].connectedBody);
				for(int k=0; k < neighbours[i].connectedBody.transform.childCount; k++)
				{
					ignoredPoints.Add(neighbours[i].connectedBody.transform.GetChild(k));
				}
				
				//call recursivly for his neighbours
				addNeighbours(neighbours[i].gameObject.GetComponents<FixedJoint>(), visitedNodes);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//apply forces for attraction if we cannot connect to anearby trinagle
		applyForces();	
	}
	

	/**
	 * @param Transform CollisionObj is the colliding triangle.
	 * If there exists triangle attacted to this one, chain through them to connected them to grid
	 */
	void formConnection(Transform collisionObj)
	{
		if(enabled)
		{
		
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			gridScript.connectTriangle(collisionObj.gameObject, gameObject);
			enabled = false;
			GlobalFlags.canFire = true;
			
			FixedJoint[] hinges = GetComponents<FixedJoint>();
			
			for(int i=0; i<hinges.Length; i++)
			{
				Rigidbody r = hinges[i].connectedBody;
				DestroyImmediate(hinges[i]);
				
				r.GetComponent<attraction>().formConnectionChain(transform);
			}
		}
		
	}
	
	//only called from another triangle. purpose to to lock to grid when a trinangle
	//connected to this one also connected to the grid
	public void formConnectionChain(Transform collisionObj)
	{
		FixedJoint[] hinges = GetComponents<FixedJoint>();
			
		for(int i=0; i<hinges.Length; i++)
		{
			if(hinges[i].connectedBody == collisionObj)
			{
				DestroyImmediate(hinges[i]);
			}
		}
		
		formConnection(collisionObj);
	}
	
	//given our position and taget position, find smallest rotation to face that point
	float findSmallestRotation(Vector3 targetPos, Vector3 myPos, float angle)
	{
		float smallestRot = angle;
		//find polarity
		Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), transform.rotation * faceNormals[0]);
		float dotProduct = Vector3.Dot(cross, targetPos - myPos);
		bool negative = dotProduct < 0;

		for(int i=1; i < faceNormals.Length; i++)
		{
			float rotToFace = Vector3.Angle((transform.rotation * faceNormals[i]).normalized, (targetPos - myPos).normalized);

			if(smallestRot > rotToFace)
			{
				smallestRot = rotToFace;
				//find polarity
				cross = Vector3.Cross(new Vector3(0, 0, 1), transform.rotation * faceNormals[i]);
				dotProduct = Vector3.Dot(cross, targetPos - myPos);
				negative = dotProduct < 0;
			}
		}
		
		if(negative)
		{
			return -smallestRot;
		}
		else
		{
			return smallestRot;
		}
	}

	void applyForces()
	{
		System.Collections.Generic.List<PullForce> centerForces = new System.Collections.Generic.List<PullForce>();
		System.Collections.Generic.List<PullForce> lowestCenterForces = new System.Collections.Generic.List<PullForce>();

		for(int i=0; i< centerPoints.Count; i++)
		{
			centerForces.Add(getForceForPoint(centerPoints[i], false));
		}
		
		//sort to find strongest forces
		lowestCenterForces = sortForces(centerForces);

		float forceMult = 1f;
		Vector3 finalForce = Vector3.zero;
		
		//if we are close to a attraction point, attract to it. otherwise attract toward 0,0,0
		if(lowestCenterForces.Count > 0)
		{
			finalForce = lowestCenterForces[0].force;			
			if(lowestCenterForces[0].dist > 0)
			{
				forceMult = pullFactor/(Mathf.Pow(lowestCenterForces[0].dist, 2));
			}
			
		}
		else
		{
			finalForce = pullFactor*((Vector3.zero - transform.position).normalized);
		}
		
		rigidbody.AddForce(finalForce * forceMult);	
		
		
		//if we are close to a point, try to face it. otherwise try to face center
		
		Vector3 targetLookPoint = Vector3.zero;			
		float rot;
		
		if(lowestCenterForces.Count > 0)
		{
			rot = Vector3.Angle((transform.rotation * faceNormals[0]).normalized, (lowestCenterForces[0].dstObj.position - transform.position).normalized);
			rot = findSmallestRotation(lowestCenterForces[0].dstObj.position, transform.position, rot);
		}
		else
		{
			rot = Vector3.Angle((transform.rotation * faceNormals[0]).normalized, (Vector3.zero - transform.position).normalized);
			rot = findSmallestRotation(new Vector3(0, 0, 0), transform.position, rot);
		}
		
		if(rot < 0)
		{
			if(rot > Time.deltaTime * rotationSpeed)
			{
				transform.Rotate(new Vector3(0, 0, rot));
			}
			else
			{
				transform.Rotate(new Vector3(0, 0, Time.deltaTime * -rotationSpeed));
			}
		}
		else
		{
			if(rot < Time.deltaTime * rotationSpeed)
			{
				transform.Rotate(new Vector3(0, 0, rot));
			}
			else
			{
				transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
			}
		}
		
		//if we are close enough to triangle, lock to it
		if(lowestCenterForces.Count > 0)
		{
			if(Vector3.Distance(lowestCenterForces[0].dstObj.position, lowestCenterForces[0].applyObj.position) < connectionDist)
			{
				formConnection(lowestCenterForces[0].dstObj.parent);
			}
		}
	}

	/**
	 * Used to determine the force for a given point. It will find the closest corner/middle point
	 * in all of the triangles in the scene and return a pullforce struct
	 * @param Transform is the transform point that we want to determine the pull force it has
	 * @param bool is to say it it's a corner point or not. 
	 */
	PullForce getForceForPoint(Transform o, bool cornerPoint)
	{
		GameObject[] points;
		if(cornerPoint)
		{
			points = GameObject.FindGameObjectsWithTag("CornerControlPoint");
		}
		else
		{
			points = GameObject.FindGameObjectsWithTag("MiddleControlPoint");
		}
		 
		
		Transform closestPoint = o;
		double dist = -1;
		
		for(int i=0; i< points.Length; i++)
		{
			double newDist = Vector3.Distance(o.position, points[i].transform.position) + 0.001f;
			if(!shouldIgnore(points[i]) && ((dist == -1) || (newDist< dist)))
			{
				closestPoint = points[i].transform;
				dist = newDist;
			}
		}
		
		if(dist < attractionDistance && dist != -1)
		{
			Vector3 force = (closestPoint.position - o.position);
			
			return new PullForce(o, closestPoint, force);
		}
		
		// return max force so it is never chosen as the lowest force. this is the equivlant to null
		return new PullForce(transform, transform, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue )); 
	}

	/**
	 * Used to determine if this point is a child of the triangle we're currently in
	 * @param Gameobject the point which we compare. 
	 */
	bool shouldIgnore(GameObject o)
	{
		if(o.transform.IsChildOf(transform))
		{
			return true;
		}
		
		for(int i=0; i<ignoredPoints.Count; i++)
		{
			if(ignoredPoints[i] == o.transform)
			{
				return true;
			}
		}
		
		return false;
	}
	
	// use selection sort to return a sort list of the pull forces
	// sorted from smallest to largest
	System.Collections.Generic.List<PullForce> sortForces(System.Collections.Generic.List<PullForce> a)
	{
		//remove any null entries
		for(int k = 0; k< a.Count; k++)
		{
			if(a[k].force == new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
			{
				a.RemoveAt(k);
				k--;
			}
		}
		
		//sort list
		int i,j;
		int iMin;
		 
		for (j = 0; j < a.Count - 1; j++) {
		    iMin = j;
		    for ( i = j+1; i < a.Count; i++) {
		        if (a[i].dist < a[iMin].dist) {
		            iMin = i;
		        }
		    }

		    if ( iMin != j ) {
				PullForce temp = a[j];
				a[j] = a[iMin];
		       	a[iMin] = temp;
		    }
		}
		
		return a;
	}
}
