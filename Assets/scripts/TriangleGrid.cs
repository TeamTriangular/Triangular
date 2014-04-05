using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriangleGrid : MonoBehaviour {

	public GameObject rootTriangle;
	public GameObject trianglePrefab;
	public GameObject triangleOutlinePrefab;
	public bool doShowGrid;
	public bool inEditMode;

	int gridSize = 0;
	// use getNode to access. uses euclidian coordinates. 0,0 is th center triangle
	private triangleNode[,] grid;
	private System.Collections.Generic.List<triangleNode> gridOutline;

	//This is used because we temporarily need to remove the center from the grid
	//when cascading to avoid infinit recursion
	private triangleNode tempStoreCenter;
	
	//delay updating attraction points until next update tick
	bool updateControlPoints = false;
	
	//used for cascading
	private System.Collections.Generic.Stack<triangleNode> chainedClusters = new System.Collections.Generic.Stack<triangleNode>();
	private float elapsedChainDelay = float.MaxValue;
	public float chainDelayTime = 2.0f;

	Music music;

	private class triangleNode
	{
		public GameObject triangleObject;
		public int x, y;
		public bool delayedDestroy;
		public bool skipDelay;

		public triangleNode(GameObject obj, int x_, int y_)
		{
			triangleObject = obj;
			x = x_;
			y = y_;
			delayedDestroy = false;
			skipDelay = false;
		}
	};

	// Use this for initialization
	void Start () 
	{
		GameObject mGameObject = GameObject.FindGameObjectWithTag("Music");
		music = mGameObject.GetComponent<Music> ();

		GlobalFlags.resetMultiplier();
		GlobalFlags.setScore(0);
		grid = new triangleNode[20,20];
		gridSize = (int)Mathf.Sqrt(grid.Length);
		gridOutline = new System.Collections.Generic.List<triangleNode>();
		
		for(int i=0; i<gridSize; i++)
		{
			for(int j=0; j<gridSize; j++)
			{
				grid[i,j] = null;
			}
		}

		setNode(0, 0, rootTriangle);

		if(doShowGrid)
		{
			showGrid();
		}
		if(!inEditMode)
		{
			loadLevel(1);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(inEditMode && Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray,out hitInfo))
			{
				if(hitInfo.collider.gameObject.tag == "gridPanel")
				{
					triangleNode node = findOutlineNode(hitInfo.collider.gameObject.transform.parent.gameObject);
					if(node != null && GetComponent<HandleUI>().IsAdding())
					{
						string c = GetComponent<HandleUI>().GetColourString();
						Vector2 realCoords = getRealCoords(node.x, node.y);
						if(grid[(int)realCoords.x,(int)realCoords.y] == null)
						{
							createTriangleOnGrid(node.x, node.y, c);
						}
					}
				}
				else if(hitInfo.collider.gameObject.tag == "Triangle")
				{
					triangleNode node = getNode(hitInfo.collider.gameObject);
					if(node != null && !GetComponent<HandleUI>().IsAdding()
					   && !(node.x == 0 && node.y == 0))
					{
						Vector2 realCoords = getRealCoords(node.x, node.y);
						grid[(int)realCoords.x,(int)realCoords.y] = null;
						Destroy(hitInfo.collider.gameObject);
					}
				}
			}
		}

		GameObject[] triangles = GameObject.FindGameObjectsWithTag("Triangle");
		GameObject gObject = GameObject.FindGameObjectWithTag("Queue");

		queue queueScript = gObject.GetComponent<queue>();

		if(updateControlPoints)			
		{
			updateAllAttractionPoints();
			updateControlPoints = false;
			
			//if all triangles are atatched to grid, let the player fire again
			if(allTrianglesStatic())
			{
				GlobalFlags.trianglesStatic = true;
				if (queueScript.trisLeftInQueue() == 0 && triangles.Length != 1) {
					music.audio.volume = 0.1f;
					music.setSeStartTime(Time.time, 2);
					music.playSoundEffect("gameOver");
					Application.LoadLevel("EndGameMenu");
				}
			}
		}

		if(!inEditMode)
		{
			GameObject[] queueTriangles = GameObject.FindGameObjectsWithTag("QueueTriangle");
			GlobalFlags.setQueueBonusTotal(queueTriangles.Length * GlobalFlags.getQueueBounus());

			//decrement delay time
			if(((elapsedChainDelay > 0 && elapsedChainDelay != float.MaxValue)) && (chainedClusters.Count == 0 || (chainedClusters.Count > 0 && !chainedClusters.Peek().skipDelay)))
			{
				elapsedChainDelay -= Time.deltaTime;
			}
			else if (elapsedChainDelay != float.MaxValue)
			{
				elapsedChainDelay = float.MaxValue;
			
				//if clusters left to deal with, color them and chain more
				if(chainedClusters.Count > 0)
				{
					GlobalFlags.incrementMultiplier();
					CheckForGreaterTriangle(chainedClusters.Pop());
					startChainDelay();
				
				}
				else if(chainedClusters.Count == 0)
				{
		
					foreach (triangleNode n in grid)
					{
						if(n != null && n.delayedDestroy && n.triangleObject.GetComponent<TriangleColour>().GetColour() != Color.black)
						{
							Destroy(n.triangleObject);
							deleteNode(n.x, n.y);
						}
						else if(n != null) // if it is black, set to not deleted
						{
							n.delayedDestroy = false;
						}
					}
					
					//Remove any triangles stranded by this action
					dettatchStranded();
					
					updateControlPoints = true;
						
					GlobalFlags.resetMultiplier();	
				}		
			}
			else
			{
				if(triangles.Length == 1)
				{
					music.audio.volume = 0.1f;
					music.setSeStartTime(Time.time, 6);
					music.playSoundEffect("machoMadness");
					GlobalFlags.setScore(GlobalFlags.getScore() + GlobalFlags.getQueueBounusTotal());
					Application.LoadLevel("PostGameMenu");
				}
			}
		}
	}
	
	private void startChainDelay()
	{
		elapsedChainDelay = chainDelayTime;
	}
	
	public bool isAttracting(GameObject o)
	{
		if(!isStatic(o))
		{
			return false;
		}
		
		// and not surrounded
		bool innerTriangle = true;
		triangleNode thisNode = getNode(o);
		Vector2[] possibleoffsets = {new Vector2(-1,0), new Vector2(1,0), new Vector2(0,1)};
		if(isPointingUp(thisNode))
		{
			possibleoffsets[2] = new Vector2(0, -1);
		}
		
		for(int k=0; k< possibleoffsets.Length; k++)
		{
			if(getNode((int)possibleoffsets[k].x + thisNode.x, (int)possibleoffsets[k].y + thisNode.y) == null)
			{
				innerTriangle = false;
			}
		}
		
		return !innerTriangle;
	}
	
	public bool allTrianglesStatic()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Triangle");
		
		foreach( GameObject n in objs)
		{	
			//dont add ourselves
			if(!isStatic(n))
			{
				return false;
			}
		}
		
		return true;
	}
	
	public bool isStatic(GameObject o)
	{
		for(int i=0; i<gridSize; i++)
		{
			for(int j=0; j<gridSize; j++)
			{
				if(grid[i, j] != null && grid[i, j].triangleObject == o)
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	//return points that should not be used to attract to(i.e triangles that re no connected to the grid)
	public System.Collections.Generic.List<Transform> getAttractionPoints(GameObject o)
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Triangle");
		
		System.Collections.Generic.List<Transform> attraction = new System.Collections.Generic.List<Transform>();
		
		foreach( GameObject n in objs)
		{	
			//dont add ourselves
			if(n != o)
			{
				//if not attatched to the center, add to ignore vector
				if(isAttracting(n))
				{
					for(int k=0; k < n.transform.childCount; k++)
					{
						attraction.Add(n.transform.GetChild(k));
					}
				}
			}
		}
		
		return attraction;
	}
	
	//updates all triangles of attraction points
	void updateAllAttractionPoints()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Triangle");
		
		foreach( GameObject n in objs)
		{	
			if(!isStatic(n))
			{
				n.GetComponent<attraction>().updateAttractionPoints();
			}
		}
	}
	
	public void showGrid()
	{
		for(int i = -gridSize/2 + 1;i < gridSize/2; i++)
		{
			for(int j = -gridSize/2 + 1;j < gridSize/2; j++)
			{
				GameObject tempObj = (GameObject)Instantiate(triangleOutlinePrefab);
				//DestroyImmediate(temp.GetComponent<attraction>());
				//temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				
				triangleNode tempNode = new triangleNode(tempObj, i, j);
				gridOutline.Add(tempNode);

				setCorrectPosition(tempNode);
			}
		}
	}
	
	//loads level x where the name of the file is levelx
	public void loadLevel(int index)
	{
		//color not being used yet
		//TODO createTriangleOnGrid needs a extra perameter and use color for deciding which color of triangle to instantiate
		LevelParser parser = GameObject.FindGameObjectWithTag ("GameManager").GetComponentInChildren<LevelParser>(); 
		LevelParser.TriInfo [] triArray = parser.getTriArray ();

		for (int i = 0; i < triArray.Length; i++) {
			LevelParser.TriInfo temp = triArray[i];
			createTriangleOnGrid(temp.getX(), temp.getY(), temp.getColour());
		}
	}
	
	public void hideGrid()
	{
		for(int i=0; i< gridOutline.Count; i++)
		{
			DestroyImmediate(gridOutline[i].triangleObject);
		}
		
		gridOutline.Clear();
	}

	private triangleNode findOutlineNode(GameObject obj)
	{
		for(int i=0; i< gridOutline.Count; i++)
		{
			if(gridOutline[i].triangleObject.Equals(obj))
			{
				return gridOutline[i];
			}
		}
		return null;
	}
	
	//adds a triangle to the grid. used for the prexisting triangles of a level
	private void createTriangleOnGrid(int x, int y, string colour)
	{
		GameObject tempObj = (GameObject)Instantiate(trianglePrefab);
		tempObj.GetComponent<attraction>().enabled = false;
		tempObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		tempObj.GetComponent<TriangleColour>().SetColourFromString(colour);

		setNode(x, y, tempObj);
		setCorrectPosition(getNode(x, y));
	}
	
	Vector2 getBestPlaceOnGrid(GameObject oldTriangle, GameObject newTriangle)
	{
		triangleNode oldNode = getNode(oldTriangle);

		if(oldNode == null)
		{
			Debug.Log("could not find stationary triangle to attatch triange to");
			return new Vector2(0, 0);
		}

		//find the position closest to the new triangle around the possible locations for the new triangle to attatch
		//default to right
		int x = int.MaxValue,y = int.MaxValue;
		float minDist = float.MaxValue;

		Vector2[] possibleoffsets = {new Vector2(-1,0), new Vector2(1,0), new Vector2(0,1)};

		if(isPointingUp(oldNode))
		{
			possibleoffsets[2] = new Vector2(0, -1);
		}
		
		for(int i=0; i< possibleoffsets.Length; i++)
		{
			if(getNode(oldNode.x + (int)possibleoffsets[i].x, oldNode.y + (int)possibleoffsets[i].y) == null &&(
			   minDist > Vector3.Distance(newTriangle.transform.position, getBasePosition(oldNode.x + (int)possibleoffsets[i].x, oldNode.y + (int)possibleoffsets[i].y)) 
				|| (x == int.MaxValue || y == int.MaxValue)))
			{
				x = oldNode.x + (int)possibleoffsets[i].x;
				y = oldNode.y + (int)possibleoffsets[i].y;
				minDist = Vector3.Distance(newTriangle.transform.position, getBasePosition(x, y));
			}
		}

		if(x == int.MaxValue || y == int.MaxValue)
		{
			Debug.Log("all sides of the base triangle are taken. not able to attatch triangle");
			return new Vector2(0, 0);
		}
		
		
		return new Vector2(x, y);
	}
	
	public Vector3 closestAvailablePos(GameObject oldTriangle, GameObject newTriangle)
	{
		Vector2 gridPos = getBestPlaceOnGrid(oldTriangle, newTriangle);
		
		return getBasePosition((int)gridPos.x, (int)gridPos.y);
	}

	public Vector3 closestAvailableRot(GameObject oldTriangle, GameObject newTriangle)
	{
		Vector2 gridPos = getBestPlaceOnGrid(oldTriangle, newTriangle);
		
		return getBaseRot((int)gridPos.x, (int)gridPos.y);
	}
	
	//attatches new triangle to old triangle
	public void connectTriangle(GameObject oldTriangle, GameObject newTriangle)
	{

		Vector2 gridPos = getBestPlaceOnGrid(oldTriangle, newTriangle);

		//attatch the new triangle to the side of the oldtriangle closest
		setNode((int)gridPos.x, (int)gridPos.y, newTriangle);
		setCorrectPosition(getNode((int)gridPos.x, (int)gridPos.y));
		//checking for a Greater Triangle
		if(!CheckForGreaterTriangle(getNode((int)gridPos.x, (int)gridPos.y)))
		{
			updateControlPoints = true;
		}
		
	}

	/// <summary>
	/// Checks for greater triangle.
	/// </summary>
	/// <param name="justAdded">Triangle that was just added</param>
	private bool CheckForGreaterTriangle(triangleNode justAdded)
	{
		triangleNode n;

		if(isPointingUp(justAdded))
		{
			//check upper node
			n = getNode(justAdded.x, justAdded.y - 1);
			if(n !=null && CompareTriangleColours(n) && !n.delayedDestroy)
			{
				SetGreaterTriangleColours(n);
				return true;
			}
		}
		else
		{
			//check lower node
			n = getNode(justAdded.x, justAdded.y + 1);
			if(n !=null && CompareTriangleColours(n) && !n.delayedDestroy)
			{
				SetGreaterTriangleColours(n);
				return true;
			}
		}
		
		
		//check left node
		n = getNode(justAdded.x - 1, justAdded.y);
		if(n !=null && CompareTriangleColours(n) && !n.delayedDestroy)
		{
			SetGreaterTriangleColours(n);
			return true;
		}
		
		//check right node
		n = getNode(justAdded.x + 1, justAdded.y);
		if(n !=null && CompareTriangleColours(n) && !n.delayedDestroy)
		{
			SetGreaterTriangleColours(n);
			return true;
		}
		
		return false;
	}
	
	//input is a triangle on the outsde of a cluster. if a cluster exists, return the node that is the center of that cluster. else null
	private triangleNode getCenterOfNewCluster(triangleNode center)
	{
		triangleNode n;
		if(isPointingUp(center))
		{
			//check upper node
			n = getNode(center.x, center.y - 1);
			if(n != null && CompareTriangleColours(n) && !n.delayedDestroy)
			{
				return n;
			}
		}
		else
		{
			n = getNode(center.x, center.y + 1);
			if(n != null && CompareTriangleColours(n) && !n.delayedDestroy)
			{
				return n;
			}
		}
		
		n = getNode(center.x + 1, center.y);
		if(n != null && CompareTriangleColours(n) && !n.delayedDestroy )
		{
			return n;
		}
		
		n = getNode(center.x - 1, center.y);
		if(n != null && CompareTriangleColours(n) && !n.delayedDestroy)
		{
			return n;
		}
		
		return null;
	}
	
	private bool skipNewCluster(triangleNode center)
	{
		Vector2[] possibleoffsets = {new Vector2(-1,0), new Vector2(1,0), new Vector2(0,1)};
		if(isPointingUp(center))
		{
			possibleoffsets[2] = new Vector2(0, -1);
		}
		
		for(int k=0; k< possibleoffsets.Length; k++)
		{
			triangleNode neighbour = getNode((int)possibleoffsets[k].x + center.x, (int)possibleoffsets[k].y + center.y);
			if(neighbour != null &&
				center.triangleObject.GetComponent<TriangleColour>().GetColour() != neighbour.triangleObject.GetComponent<TriangleColour>().GetColour())
			{
				return false;
			}
		}
		
		return true;
	}

	/// <summary>
	/// looks at the 3 surrounding triangles and ensures they have the 
	/// same colour
	/// </summary>
	/// <returns><c>true</c>, if triangle colours are the same
	/// <c>false</c> otherwise.</returns>
	/// <param name="center">The triangle in the center</param>
	private bool CompareTriangleColours(triangleNode center)
	{
		Color c = new Color();
		triangleNode n;

		if(isPointingUp(center))
		{
			//check upper node
			n = getNode(center.x, center.y - 1);
			if(n == null)
			{
				return false;
			}
			c = (n.triangleObject.GetComponent<TriangleColour>().GetColour());
		}
		else
		{
			//check lower node
			n = getNode(center.x, center.y + 1);
			if(n == null)
			{
				return false;
			}
			c = (n.triangleObject.GetComponent<TriangleColour>().GetColour());
		}
		
		//check left node
		n = getNode(center.x - 1, center.y);
		if(n != null)
		{
			if( c == Color.black)
			{
				c = (n.triangleObject.GetComponent<TriangleColour>().GetColour());
			}
			else
			{
				if(c != (n.triangleObject.GetComponent<TriangleColour>().GetColour())
				   && (n.triangleObject.GetComponent<TriangleColour>().GetColour()) != Color.black)
				{
					return false;
				}
			}
		}
		else
		{
			return false;
		}
		
		//check right node
		n = getNode(center.x + 1, center.y);
		if(n != null)
		{
			if(c != (n.triangleObject.GetComponent<TriangleColour>().GetColour())
			   && (n.triangleObject.GetComponent<TriangleColour>().GetColour()) != Color.black)
			{
				return false;
			}
		}
		else
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Sets the greater triangle colours to match the center colour
	/// Also increments score
	/// </summary>
	/// <param name="center">The Center Triangle</param>
	private void SetGreaterTriangleColours(triangleNode center)
	{
		GlobalFlags.setScore(GlobalFlags.getScore() + (GlobalFlags.getBaseScoreValue() * GlobalFlags.getMultiplier()));

		center.delayedDestroy = true;
		
		Color c = center.triangleObject.GetComponent<TriangleColour>().GetColour();
		triangleNode n;
		
		if( c != Color.black)
		{
			if(isPointingUp(center))
			{
				//check upper node
				n = getNode(center.x, center.y - 1);
				if(n !=null && 
				   n.triangleObject.GetComponent<TriangleColour>().GetColour() 
				   != Color.black)
				{
					n.triangleObject.GetComponent<TriangleColour>().SetColour(c);
				}
			}
			else
			{
				//check lower node
				n = getNode(center.x, center.y + 1);
				if(n !=null && 
				   n.triangleObject.GetComponent<TriangleColour>().GetColour() 
				   != Color.black)
				{
					n.triangleObject.GetComponent<TriangleColour>().SetColour(c);
				}
			}
		
			//check left node
			n = getNode(center.x - 1, center.y);
			if(n !=null && 
			   n.triangleObject.GetComponent<TriangleColour>().GetColour() 
			   != Color.black)
			{
				n.triangleObject.GetComponent<TriangleColour>().SetColour(c);
			}
		
			//check right node
			n = getNode(center.x + 1, center.y);
			if(n !=null && 
			   n.triangleObject.GetComponent<TriangleColour>().GetColour() 
			   != Color.black)
			{
				n.triangleObject.GetComponent<TriangleColour>().SetColour(c);
			}
		}
		
		CascadeAndClear(center);


	}

	private void CascadeAndClear(triangleNode center)
	{
		bool isUpwards = isPointingUp(center);
		int x = center.x;
		int y = center.y;
		Vector2 realCords = getRealCoords(x , y);
		triangleNode n;

		//destroy the center
		center.delayedDestroy = true;	

		if(isUpwards)
		{
			n = getNode(x, y - 1);
			if(n != null &&	
			   n.triangleObject.GetComponent<TriangleColour>().GetColour()
			   != Color.black)
			{
				n.delayedDestroy = true;
				triangleNode newCenter = getCenterOfNewCluster(n);
				if(newCenter != null)
				{
					if(skipNewCluster(newCenter))
					{
						//n.skipDelay = true;
					}
					chainedClusters.Push(n);
				}
			}
		}
		else
		{
			n = getNode(x, y + 1);
			if(n != null &&	
			   n.triangleObject.GetComponent<TriangleColour>().GetColour()
			   != Color.black)
			{
				n.delayedDestroy = true;
				triangleNode newCenter = getCenterOfNewCluster(n);
				if(newCenter != null)
				{
					if(skipNewCluster(newCenter))
					{
						//n.skipDelay = true;
					}
					chainedClusters.Push(n);
				}
			}
		}



		// Left Node
		n = getNode(x - 1, y);
		if(n != null &&	
		   n.triangleObject.GetComponent<TriangleColour>().GetColour()
		   != Color.black)
		{
			n.delayedDestroy = true;
			triangleNode newCenter = getCenterOfNewCluster(n);
			if(newCenter != null)
			{
				if(skipNewCluster(newCenter))
				{
					//n.skipDelay = true;
				}
				chainedClusters.Push(n);
			}
		}

		// Right Node
		n = getNode(x + 1, y);
		
		if(n != null &&	
		   n.triangleObject.GetComponent<TriangleColour>().GetColour()
		   != Color.black)
		{
			n.delayedDestroy = true;
			triangleNode newCenter = getCenterOfNewCluster(n);
			if(newCenter != null)
			{
				if(skipNewCluster(newCenter))
				{
					//n.skipDelay = true;
				}
				chainedClusters.Push(n);
			}
		}
		

		startChainDelay();
	}

	/// <summary>
	/// Checks for any nodes not connected to the center and removes them
	/// </summary>
	private void dettatchStranded()
	{
		
		triangleNode center = getNode(0, 0);
		//remeber stranded triangles
		System.Collections.Generic.List<triangleNode> strandedTriangles = new System.Collections.Generic.List<triangleNode>();
		foreach( triangleNode n in grid)
		{	
			if(n != null)
			{
				if(!AStar(n,center))
				{
					strandedTriangles.Add(n);
				}
			}
		}
		
		foreach( triangleNode n in strandedTriangles)
		{
			if(n.triangleObject.GetComponent<attraction>() == null)
			{
				n.triangleObject.AddComponent<attraction>();
			}
			
			deleteNode(n.x, n.y);

			n.triangleObject.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
		}
		
		foreach( triangleNode n in strandedTriangles)
		{			
			n.triangleObject.GetComponent<attraction>().updateAttractionPoints();
			if(n.triangleObject.GetComponents<FixedJoint>().Length < 3)
			{
				n.triangleObject.GetComponent<attraction>().enabled = true;
			}
		}
	}
	
	void connectToNeighbours(triangleNode n)
	{
		Vector2[] possibleoffsets = {new Vector2(-1,0), new Vector2(1,0), new Vector2(0,1)};

		if(isPointingUp(n))
		{
			possibleoffsets[2] = new Vector2(0, -1);
		}
		
		for(int i=0; i < possibleoffsets.Length; i++)
		{
			if(getNode((int)(n.x + possibleoffsets[i].x), (int)(n.y + possibleoffsets[i].y)) != null)
			{
				triangleNode t = getNode((int)(n.x + possibleoffsets[i].x), (int)(n.y + possibleoffsets[i].y));
				FixedJoint f = n.triangleObject.AddComponent<FixedJoint>();
				f.connectedBody = t.triangleObject.GetComponent<Rigidbody>();
			}
		}
	}

	/// <summary>
	/// Searches for a path from start to goal
	/// </summary>
	/// <returns><c>true</c> if a path exists <c>false</c> otherwise.</returns>
	/// <param name="start">Starting node</param>
	/// <param name="goal">Goal node</param>
	private bool AStar(triangleNode start, triangleNode goal)
	{
		IList<triangleNode> closedSet = new List<triangleNode>();
		IList<triangleNode> openSet = new List<triangleNode>();
		openSet.Add(start);
		triangleNode current;
		triangleNode n;

		while (openSet.Count != 0)
		{
			current = openSet[0];

			if (current.Equals(goal))
			{
				return true;
			}
			
			closedSet.Add(current);
			openSet.Remove(current);

			if(isPointingUp(current))
			{
				if(current.y - 1 > -gridSize/2)
				{
					n = getNode(current.x, current.y - 1);
					if ( n != null && !closedSet.Contains(n))
					{
						openSet.Add(n);
					}
				}
			}
			else
			{
				if(current.y + 1 < gridSize/2)
				{
					n = getNode(current.x, current.y + 1);
					if ( n != null && !closedSet.Contains(n))
					{
						openSet.Add(n);
					}
				}
			}
			//check left
			if(current.x - 1 > -gridSize/2)
			{
				n = getNode(current.x - 1, current.y);
				if ( n != null && !closedSet.Contains(n))
				{
					openSet.Add(n);
				}
			}
			//check right
			if(current.x + 1 < gridSize/2)
			{
				n = getNode(current.x + 1, current.y);
				if ( n != null && !closedSet.Contains(n))
				{
					openSet.Add(n);
				}
			}
		}
		return false;
	}

	private triangleNode getNode(int x, int y)
	{
		Vector2 coords = getRealCoords(x, y);

		return grid[(int)coords.x, (int)coords.y];
	}

	private triangleNode getNode(GameObject obj)
	{
		for(int i=0; i<gridSize; i++)
		{
			for(int j=0; j<gridSize; j++)
			{
				if(grid[i, j] != null && grid[i, j].triangleObject == obj)
				{
					return grid[i, j];
				}
			}
		}

		//return null if not found
		return null;
	}

	private void setNode(int x, int y , GameObject obj)
	{
		Vector2 coords = getRealCoords(x, y);

		grid[(int)coords.x ,(int)coords.y] = new triangleNode(obj, x, y);
	}
	
	private void deleteNode(int x, int y)
	{
		Vector2 coords = getRealCoords(x, y);

		grid[(int)coords.x ,(int)coords.y] = null;
	}

	private void setNode(Vector2 coords , GameObject obj)
	{
		setNode((int)coords.x, (int)coords.y, obj);
	}

	private Vector2 getRealCoords(int x_, int y_)
	{
		int x = gridSize/2 + x_;
		int y = gridSize/2 - y_;

		return new Vector2(x, y);
	}

	public void setCorrectPosition(GameObject o)
	{
		setCorrectPosition(getNode(o));
	}

	private void setCorrectPosition(triangleNode n)
	{
		Vector3 pos = getBasePosition(n.x, n.y);
	
		n.triangleObject.transform.rotation = Quaternion.Euler(getBaseRot(n.x, n.y));	
		n.triangleObject.transform.position = pos;
	}	
	
	private Vector3 getBaseRot(int x, int y)
	{
		if(isPointingUp(x, y))
		{			
			return new Vector3(0, 0, 0);
		}
		else
		{	
			return new Vector3(0, 0, 180);
		}
	}
	
	private Vector3 getBasePosition(int x, int y)
	{
		float xOffset = x * 0.5f ;
		float yOffset = y * 0.866f;

		return new Vector3(xOffset, yOffset, 0);
	}

	private bool isPointingUp(triangleNode n)
	{
		return isPointingUp(n.x, n.y);
	}

	private bool isPointingUp(int x, int y)
	{
		return Mathf.Abs(x % 2) == Mathf.Abs(y % 2);
	}

	public string[,] toStringArray()
	{
		string [,] array = new string[gridSize,gridSize];
		for(int i = 0; i < gridSize; i++)
		{
			for(int j = 0; j < gridSize; j++)
			{
				triangleNode n = grid[i,j];
				if(n == null)
				{
					array[i,j] = "";
				}
				else
				{
					array[i,j] = n.triangleObject.GetComponent<TriangleColour>()
						.GetCurrentColourAsString();
				}
			}
		}
		return array;
	}

	//for debugging purposes
	public string toString()
	{
		string s = "";

		for(int i= gridSize/2; i > -gridSize/2; i--)
		{
			for(int j= -gridSize/2; j < gridSize/2; j++)
			{
				triangleNode n = getNode(j, i);
				if(n != null)
				{
					s += "0";
				}
				else
				{
					s += "1";
				}
			}

			s += "\n";
		}

		return s;
	}
}
