using UnityEngine;
using System.Collections;

public class TriangleGrid : MonoBehaviour {

	public GameObject rootTriangle;
	public GameObject trianglePrefab;
	public GameObject triangleOutlinePrefab;

	int gridSize = 0;
	// use getNode to access. uses euclidian coordinates. 0,0 is th center triangle
	private triangleNode[,] grid;
	private System.Collections.Generic.List<triangleNode> gridOutline;
	

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
		try 
	    {
			string[] lines = System.IO.File.ReadAllLines("assets/levels/level" + index  + ".txt");
			
			for(int i=0; i< lines.Length; i++)
			{
				int commaIndex = lines[i].IndexOf(',');
				int colonIndex = lines[i].IndexOf(':');
				int x = int.Parse(lines[i].Substring(0, commaIndex));
				int y = int.Parse(lines[i].Substring(commaIndex + 1, colonIndex - 1 - commaIndex));
				
				//color not being used yet
				//createTriangleOnGrid needs a extra perameter and use color for deciding which color of triangle to instantiate
				string color = lines[i].Substring(colonIndex + 1, lines[i].Length - 1 - colonIndex);
		
				createTriangleOnGrid(x, y);
			
			}
		}
		catch
		{
			Debug.Log("error reading file level" + index + ".txt");
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

	//attatches new triangle to old triangle
	public void connectTriangle(GameObject oldTriangle, GameObject newTriangle)
	{

		triangleNode oldNode = getNode(oldTriangle);

		if(oldNode == null)
		{
			Debug.Log("could not find stationary triangle to attatch triange to");
			return;
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
			return;
		}

		//attatch the new triangle to the side of the oldtriangle closest
		setNode(x, y, newTriangle);
		setCorrectPosition(getNode(x, y));
		//checking for a Greater Triangle
		CheckForGreaterTriangle(getNode(x, y));
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

		//if it should be pointing down, rotate by 180
		if(isPointingUp(n))
		{			
			n.triangleObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
		else
		{	
			n.triangleObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
		}
		
		n.triangleObject.transform.position = pos;
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

		for(int i= gridSize/2; i >= -gridSize/2; i--)
		{
			for(int j= -gridSize/2; j <= gridSize/2; j++)
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
