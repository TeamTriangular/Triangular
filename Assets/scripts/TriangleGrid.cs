using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriangleGrid : MonoBehaviour {

	public GameObject rootTriangle;
	public GameObject trianglePrefab;
	public GameObject triangleOutlinePrefab;

	int gridSize = 0;
	// use getNode to access. uses euclidian coordinates. 0,0 is th center triangle
	private triangleNode[,] grid;
	private System.Collections.Generic.List<triangleNode> gridOutline;

	//This is used because we temporarily need to remove the center from the grid
	//when cascading to avoid infinit recursion
	private triangleNode tempStoreCenter;

	private class triangleNode
	{
		public GameObject triangleObject;
		public int x, y;

		public triangleNode(GameObject obj, int x_, int y_)
		{
			triangleObject = obj;
			x = x_;
			y = y_;
		}
	};

	// Use this for initialization
	void Start () 
	{
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
		
		loadLevel(1);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
			createTriangleOnGrid(temp.getX(), temp.getY());
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
	
	//adds a triangle to the grid. used for the prexisting triangles of a level
	private void createTriangleOnGrid(int x, int y)
	{
		GameObject tempObj = (GameObject)Instantiate(trianglePrefab);
		tempObj.GetComponent<attraction>().enabled = false;
		tempObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		
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
		CheckForGreaterTriangle(getNode((int)gridPos.x, (int)gridPos.y));
	}

	/// <summary>
	/// Checks for greater triangle.
	/// </summary>
	/// <param name="justAdded">Triangle that was just added</param>
	private void CheckForGreaterTriangle(triangleNode justAdded)
	{
		triangleNode n;

		if(isPointingUp(justAdded))
		{
			//check upper node
			n = getNode(justAdded.x, justAdded.y - 1);
			if(n !=null && CompareTriangleColours(n))
			{
				SetGreaterTriangleColours(n);
				return;
			}
		}
		else
		{
			//check lower node
			n = getNode(justAdded.x, justAdded.y + 1);
			if(n !=null && CompareTriangleColours(n))
			{
				SetGreaterTriangleColours(n);
				return;
			}
		}
		
		//check left node
		n = getNode(justAdded.x - 1, justAdded.y);
		if(n !=null && CompareTriangleColours(n))
		{
			SetGreaterTriangleColours(n);
			return;
		}
		
		//check right node
		n = getNode(justAdded.x + 1, justAdded.y);
		if(n !=null && CompareTriangleColours(n))
		{
			SetGreaterTriangleColours(n);
			return;
		}
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
	/// </summary>
	/// <param name="center">The Center Triangle</param>
	private void SetGreaterTriangleColours(triangleNode center)
	{
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

		if(center.triangleObject.GetComponent<TriangleColour>().GetColour() 
		   != Color.black)
		{
			Destroy (center.triangleObject);
			grid[(int)realCords.x, (int)realCords.y] = null;
		}
		else
		{
			//temporarily remove the center from the grid
			tempStoreCenter = grid[(int)realCords.x, (int)realCords.y];
			grid[(int)realCords.x, (int)realCords.y] = null;
		}

		if(isUpwards)
		{
			n = getNode(x, y - 1);
			if(n != null &&	
			   n.triangleObject.GetComponent<TriangleColour>().GetColour()
			   != Color.black)
			{
				CheckForGreaterTriangle(n);
				Destroy (n.triangleObject);
				realCords = getRealCoords(n.x , n.y);
				grid[(int)realCords.x, (int)realCords.y] = null;
			}
		}
		else
		{
			n = getNode(x, y + 1);
			if(n != null &&	
			   n.triangleObject.GetComponent<TriangleColour>().GetColour()
			   != Color.black)
			{
				CheckForGreaterTriangle(n);
				Destroy (n.triangleObject);
				realCords = getRealCoords(n.x , n.y);
				grid[(int)realCords.x, (int)realCords.y] = null;
			}
		}

		// Left Node
		n = getNode(x - 1, y);
		if(n != null &&	
		   n.triangleObject.GetComponent<TriangleColour>().GetColour()
		   != Color.black)
		{
			CheckForGreaterTriangle(n);
			Destroy (n.triangleObject);
			realCords = getRealCoords(n.x , n.y);
			grid[(int)realCords.x, (int)realCords.y] = null;
		}

		// Right Node
		n = getNode(x + 1, y);
		if(n != null &&	
		   n.triangleObject.GetComponent<TriangleColour>().GetColour()
		   != Color.black)
		{
			CheckForGreaterTriangle(n);
			Destroy (n.triangleObject);
			realCords = getRealCoords(n.x , n.y);
			grid[(int)realCords.x, (int)realCords.y] = null;
		}
		//Remove any triangles stranded by this action
		RemoveStranded();
	}

	/// <summary>
	/// Checks for any nodes not connected to the center and removes them
	/// </summary>
	private void RemoveStranded()
	{
		//if the center was temporarily removed, return it to the grid
		if( tempStoreCenter != null)
		{
			Vector2 centerCoord = getRealCoords(0,0);
			grid[(int)centerCoord.x, (int)centerCoord.y] = tempStoreCenter;
			tempStoreCenter = null;
		}

		triangleNode center = getNode(0, 0);
		foreach( triangleNode n in grid)
		{
			if(n != null)
			{
				if(!AStar(n,center))
				{
					Destroy (n.triangleObject);
					Vector2 realCords = getRealCoords(n.x , n.y);
					grid[(int)realCords.x, (int)realCords.y] = null;
				}
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
