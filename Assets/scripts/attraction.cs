using UnityEngine;
using System.Collections;

public class attraction : MonoBehaviour {
	public float rotationSpeed = 1.0f;

	Transform[] cornerPoints;
	Transform[] centerPoints;
	Vector3[] faceNormals = new Vector3[3];
	System.Collections.Generic.List<Connection> joints;
	public TriangleGrid gridScript;
	bool currentlyColliding = false; //true if the triangle is making contact with another triangle
	
	public struct PullForce
	{
		public Vector3 applyPos;
		public Vector3 dstPos;
		public Vector3 force;	
		public float dist;
		
		public PullForce(Vector3 applyPos_, Vector3 dstPos_, Vector3 force_)
		{
			applyPos = applyPos_;
			dstPos = dstPos_;
			force = force_;
			dist = Vector3.Distance(applyPos_, dstPos_);
		}
	};
	
	public struct Connection
	{
		public Transform ctrlPoint1;
		public Transform ctrlPoint2;
		public Transform connectedTriangle;
		
		public Connection(Transform p1, Transform p2, Transform connectedTriangle_)
		{
			ctrlPoint1 = p1;
			ctrlPoint2 = p1;
			connectedTriangle = connectedTriangle_;
		}
	};

	// Use this for initialization
	void Awake () {
		cornerPoints = new Transform[3];
		centerPoints = new Transform[3];
		
		int cornerIndex = 0;
		int centerIndex = 0;
		
		for(int i=0; i< transform.childCount; i++)
		{
			if(transform.GetChild(i).tag == "CornerControlPoint")
			{
				cornerPoints[cornerIndex] = transform.GetChild(i);
				cornerIndex++;
			}
			else if(transform.GetChild(i).tag == "MiddleControlPoint")
			{
				centerPoints[centerIndex] = transform.GetChild(i);
				centerIndex++;
			}
		}
		
		joints = new System.Collections.Generic.List<Connection>();

		gridScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TriangleGrid>();

		faceNormals[0] = Quaternion.Euler( new Vector3(0, 0, 0  )) * transform.up;
		faceNormals[1] = Quaternion.Euler( new Vector3(0, 0, 120)) * faceNormals[0];
		faceNormals[2] = Quaternion.Euler( new Vector3(0, 0, 120)) * faceNormals[1];


	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//apply forces for attraction
		applyForces();			
		
		if(joints.Count > 0)
		{
			GlobalFlags.canFire = true;
			gridScript.connectTriangle(joints[0].connectedTriangle.gameObject, gameObject);
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			enabled = false;
		}
	}
	
	void OnCollisionStay(Collision collision) 
	{
		if(collision.gameObject.tag == "Triangle" && collision.gameObject.transform.childCount >0)
		{
			for(int i=0; i< cornerPoints.Length; i++)
			{
				for(int j=0; j< (cornerPoints.Length + centerPoints.Length); j++)
				{
					//Debug.Log(collision.gameObject.transform.childCount);
					// temporarily use center points for hinge connection
					if(collision.gameObject.transform.GetChild(j).tag == "MiddleControlPoint" &&
						isCornerTouching(centerPoints[i].position, collision.gameObject.transform.GetChild(j).position))
					{
						//Debug.Log(cornerPoints[i].localPosition + "," + cornerPoints[i].position + " ||| " + collision.gameObject.transform.GetChild(j).localPosition + "," + collision.gameObject.transform.GetChild(j).position);
						attemptFormConnection(centerPoints[i].transform, collision.gameObject.transform.GetChild(j), collision.gameObject.transform);
					}
				}
			}
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Triangle")
		{
			currentlyColliding = true;
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
        if(collision.gameObject.tag == "Triangle")
		{
			currentlyColliding = false;
		}
    }

	
	void attemptFormConnection(Transform t1, Transform t2, Transform collisionObj)
	{
		for(int i=0; i< joints.Count; i++)
		{
			//if joint for the connection exists we don't want to create another
			if((joints[i].ctrlPoint1.position == t1.position) && (joints[i].ctrlPoint2.position == t2.position))
			{
				return;
			}		
			else
			{
				//Debug.Log ((joints[i].ctrlPoint1.position - t1.position) + ", " + (joints[i].ctrlPoint2.position - t2.position));
			}
		}
		
		Vector3 test = t2.position - t1.position;
		transform.position =  transform.position + test;
		
		
		//create hinge and remeber connection
		ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
		joint.connectedBody  = collisionObj.GetComponent<Rigidbody>();
		//set up limits for rotation and movement
		joint.xMotion = ConfigurableJointMotion.Limited;
		joint.yMotion = ConfigurableJointMotion.Limited;
		joint.zMotion = ConfigurableJointMotion.Limited;
		joint.angularXMotion = ConfigurableJointMotion.Free;
		joint.angularYMotion = ConfigurableJointMotion.Free;
		joint.angularZMotion = ConfigurableJointMotion.Free;
		
		joint.anchor = t1.localPosition;
		joint.connectedAnchor = t2.localPosition;
//		
//		HingeJoint joint = gameObject.AddComponent<HingeJoint>();
//		joint.connectedBody  = collisionObj.GetComponent<Rigidbody>();
//		joint.useLimits = true;
//		JointLimits limits = new JointLimits();
//		limits.min = -90;
//		limits.max = 90;
//		joint.limits = limits;
		
		joints.Add(new Connection(t1, t2, collisionObj));
	}

	void applyForces()
	{
		PullForce[] centerForces = new PullForce[3];
		PullForce[] lowestCenterForces =  new PullForce[3];
		PullForce[] cornerForces = new PullForce[3];
		PullForce[] lowestCornerForces =  new PullForce[3];

		for(int i=0; i< centerPoints.Length; i++)
		{
			centerForces[i] = getForceForPoint(centerPoints[i], false);
			cornerForces[i] = getForceForPoint(cornerPoints[i], true);
		}
		
		lowestCenterForces = sortForces(centerForces);
		lowestCornerForces = sortForces(cornerForces);

		// only force to attract triangle
		float forceMult = 1f;
		Vector3 finalForce = Vector3.zero;
		
		int numberOfCornerForces = 2;
		
		if(lowestCornerForces[0].dist < 1) //change priority of attraction point depending on distance. not done yet
		{
//			numberOfCornerForces = 1;
//			forcemult = 2;
		}
		
		for(int i=0; i< numberOfCornerForces; i++)
		{
			//finalForce += lowestCornerForces[i].force * forcemult;
		}
		
		
		for(int i=0; i< 1; i++)
		{	;
			finalForce += lowestCenterForces[i].force;
			//GetComponent<Rigidbody>().AddForce(lowestCenterForces[i].force * forcemult);	
		}
		
		if(lowestCenterForces[0].dist > 0)
		{
			forceMult = 1/(lowestCenterForces[0].dist);
		}
		
		GetComponent<Rigidbody>().AddForce(finalForce * forceMult);	

		//if we are not colliding try to rotate to face the closest triangle
		//if(!currentlyColliding || true)
		{
			float smallestRot = Vector3.Angle((transform.rotation * faceNormals[0]).normalized, (lowestCenterForces[0].dstPos - transform.position).normalized);
			//find polarity
			Vector3 cross = Vector3.Cross(new Vector3(0, 0, 1), transform.rotation * faceNormals[0]);
			float dotProduct = Vector3.Dot(cross, lowestCenterForces[0].dstPos - transform.position);
			bool negative = dotProduct < 0;

			for(int i=1; i < faceNormals.Length; i++)
			{
				float rotToFace = Vector3.Angle((transform.rotation * faceNormals[i]).normalized, (lowestCenterForces[0].dstPos - transform.position).normalized);
	
				if(smallestRot > rotToFace)
				{
					smallestRot = rotToFace;
					//find polarity
					cross = Vector3.Cross(new Vector3(0, 0, 1), transform.rotation * faceNormals[i]);
					dotProduct = Vector3.Dot(cross, lowestCenterForces[0].dstPos - transform.position);
					negative = dotProduct < 0;
				}
	
			}
	
			if(negative)
			{
				if(smallestRot < Time.deltaTime * rotationSpeed)
				{
					transform.Rotate(new Vector3(0, 0, -smallestRot));
				}
				else
				{
					transform.Rotate(new Vector3(0, 0, Time.deltaTime * -rotationSpeed));
				}
			}
			else
			{
				if(smallestRot < Time.deltaTime * rotationSpeed)
				{
					transform.Rotate(new Vector3(0, 0, smallestRot));
				}
				else
				{
					transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
				}
			}
		}
	}
	
	bool isCornerTouching(Vector3 c1, Vector3 c2)
	{
		return (Vector3.Distance(c1, c2) < 0.06f);
	}
	
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
			double newDist = Vector3.Distance(o.position, points[i].transform.position);
			if(!shouldIgnore(points[i]) && ((dist == -1) || (newDist< dist)))
			{
				closestPoint = points[i].transform;
				dist = newDist;
			}
		}
		
		//make sure it found a point that is closest
		if(dist != -1)
		{
			Vector3 force = (closestPoint.position - o.position);
			
			return new PullForce(o.position, closestPoint.position, force);
		}
		
		// return max force so it is never chosen as the lowest force. this is the equivlant to null
		return new PullForce(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue ), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue )); 
	}
	
	bool shouldIgnore(GameObject o)
	{
		if(o.transform.IsChildOf(transform))
		{
			return true;
		}
		
		return false;
	}
	
	// use selection sort to return a sort list of the pull forces
	// sorted from smallest to largest
	PullForce[] sortForces(PullForce[] a)
	{
		int i,j;
		int iMin;
		 
		for (j = 0; j < a.Length - 1; j++) {
		    iMin = j;
		    for ( i = j+1; i < a.Length; i++) {
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
