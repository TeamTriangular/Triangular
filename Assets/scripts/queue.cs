using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class queue : MonoBehaviour {

	GameObject [] inQueue;

	public GameObject trianglePrefab;

	public GameObject blueQueueTriangle;

	public GameObject aquaQueueTriangle;

	public GameObject greenQueueTriangle;

	public GameObject pinkQueueTriangle;

	public GameObject redQueueTriangle;

	public GameObject yellowQueueTriangle;

	public static readonly string RED = "r";

	public static readonly string GREEN = "g";

	public static readonly string BLUE = "b";

	public static readonly string AQUA = "a";

	public static readonly string PINK = "p";

	public static readonly string YELLOW = "y";

	public bool inEditMode;

	//distance between each triangle in the queue
	private float distance = 1.5f;

	private int currentTri = 0;

	private GameObject Manager;
	
	/**
	 * Used to load level 1 into the queue. All further levels will be checked with the update script. 
	 */
	void Start () {

		Manager = GameObject.FindGameObjectWithTag("GameManager");
		if(!inEditMode)
		{
			loadLevel();
		}
	}

	/**
	 * Called by the generator whenever a shot is fired to make sure that it can fire a shot
	 * And to let the queue know that it has to update. Will return the triangle that is to be shot,
	 * otherwise it will return null
	 */
	public GameObject fireShot() {
		if(currentTri < inQueue.Length) {
			GameObject returnTri = (GameObject) Instantiate (trianglePrefab);
			returnTri.GetComponent<TriangleColour>().SetColourFromMaterial(inQueue[currentTri].renderer.material);

			//proceed to move the triangles in the queue as one has been fired.

			for(int i = currentTri; i < inQueue.Length; i++) {
				if(i == currentTri) {
					Destroy(inQueue[i]);
				}
				else if(i < currentTri + 3) {
					inQueue[i].transform.localPosition = new Vector3((float)inQueue[i].transform.localPosition.x, 
					                                                 (float)inQueue[i].transform.localPosition.y - 1.5f,
					                                                 (float)inQueue[i].transform.localPosition.z);
				}
				else if(i == currentTri + 3) {
					inQueue[i].transform.localPosition = new Vector3((float)inQueue[i].transform.localPosition.x, 
					                                                 (float)inQueue[i].transform.localPosition.y,
					                                                 -1.5f);
				}
				else {
					break; //only need to move 4 triangles, first is destroyed next 3 are moved. 
				}
			}

			currentTri++;
			return returnTri;
		}

		return null;
	}

	/**
	 * Makes the queue longer once there are only 3 triangles left to shoot
	 * Is called after each shot and tests if the level selected is infinite and random
	 */
	public void randomLevelQueue() {
		if(GlobalFlags.infiniteRandomMode && inQueue.Length - currentTri == 3) {

			inQueue [0] = inQueue [currentTri];  
			inQueue [1] = inQueue [currentTri + 1];
			inQueue [2] = inQueue [currentTri + 2];

			for(int i = 3; i < inQueue.Length; i++) {
				GameObject colour = randomColour();
				colour.transform.localPosition = new Vector3(6.0f, 4.5f, 1.0f);
				colour.transform.localRotation = new Quaternion(0,0,0,0);
				colour.transform.localScale = new Vector3(1,1,1);
				inQueue[i] = colour;
			}
			currentTri  = 0;
		}
	}

	/**
	 * Gives a random colour that a triangle can have. If new colours are added, this function needs to be updated
	 * to attribute for this. Current triangles can be: blue, aqua, green pink, red, yellow.
	 */
	public GameObject randomColour() {
		int thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor = (int) Random.Range (1, 6);
		GameObject returnColour = null;
		
		if(thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor == 1) {
			returnColour = (GameObject) Instantiate (redQueueTriangle);
		}
		else if(thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor == 2) {
			returnColour = (GameObject) Instantiate (greenQueueTriangle);
		}
		else if(thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor == 3) {
			returnColour = (GameObject) Instantiate (aquaQueueTriangle);
		}
		else if(thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor == 4) {
			returnColour = (GameObject) Instantiate (pinkQueueTriangle);
		}
		else if(thisIsTheMostPointlesslyLongVariableNameThatHasNothingToDoWithWhatItIsUsedFor == 5) {
			returnColour = (GameObject) Instantiate (yellowQueueTriangle);
		} 
		else {
			returnColour = (GameObject) Instantiate (blueQueueTriangle);
		}
		
		return returnColour;
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
				bool QueueUpdated = false;

				if(hitInfo.collider.gameObject.tag == "Queue" 
				   && Manager.GetComponent<HandleUI>().IsAdding())
				{
					Queue<GameObject> q; 
					if (inQueue == null)
					{
						q = new Queue<GameObject>();
					}
					else
					{
						q = new Queue<GameObject>(inQueue);
					}
					q.Enqueue(AddTriangle());
					inQueue = q.ToArray();
					QueueUpdated = true;
				}
				else if(hitInfo.collider.gameObject.tag == "QueueTriangle" 
				        && !Manager.GetComponent<HandleUI>().IsAdding())
				{
					RemoveTriangle(hitInfo.collider.gameObject);
					QueueUpdated = true;
				}

				//Update Queue
				if(QueueUpdated)
				{
					float yLoc = 0.0f;
					for(int i = 0; i < inQueue.Length; i++) {
						if (i < 3) {
							yLoc += distance;
							inQueue[i].transform.localPosition = new Vector3(6.0f, yLoc, -1.5f);
						}
						else {
							inQueue[i].transform.localPosition = new Vector3(6.0f, yLoc, 1.0f);
						}
					
						inQueue[i].transform.localRotation = new Quaternion(0,0,0,0);
						inQueue[i].transform.localScale = new Vector3(1,1,1);
					}
				}
			}
		}
	}

	public int GetQueueSize()
	{
		if(inQueue == null)
		{
			return 0;
		}
		return inQueue.Length;
	}

	public int trisLeftInQueue() {
		if(inQueue == null) {
			return 0;
		}
		return inQueue.Length - currentTri;
	}

	public string[] toStringArray()
	{
		if(inQueue == null)
		{
			return new string[0];
		}
		string[] array = new string[inQueue.Length];
		for(int i = 0; i < inQueue.Length; i++)
		{
			array[i] = GetColourFromName(inQueue[i].name);
		}
		return array;
	}

	private GameObject AddTriangle()
	{
		string s = Manager.GetComponent<HandleUI>().GetColourString();

		if(s.Equals(AQUA))
		{
			return (GameObject) Instantiate(aquaQueueTriangle);
		}
		else if(s.Equals(BLUE))
		{
			return (GameObject) Instantiate(blueQueueTriangle);
		}
		else if(s.Equals(GREEN))
		{
			return (GameObject) Instantiate(greenQueueTriangle);
		}
		else if(s.Equals(RED))
		{
			return (GameObject) Instantiate(redQueueTriangle);
		}
		else if(s.Equals(YELLOW))
		{
			return (GameObject) Instantiate(yellowQueueTriangle);
		}
		else if(s.Equals(PINK))
		{
			return (GameObject) Instantiate(pinkQueueTriangle);
		}
		else
		{
			//Defaulting on blue
			return (GameObject) Instantiate(blueQueueTriangle);
		}
	}

	private void RemoveTriangle(GameObject obj)
	{
		GameObject[] tempArray = new GameObject[inQueue.Length - 1];
		int tempArrayIndex = 0;
		for(int i = 0; i < inQueue.Length; i++)
		{
			if(!inQueue[i].Equals(obj))
		 	{
				tempArray[tempArrayIndex] = inQueue[i];
				tempArrayIndex++;
			}
		}
		Destroy(obj);
		inQueue = tempArray;
	}

	/**
	 * Takes a number as an input and is called either at the creation of this script to load level 1
	 * or when a level is beaten or restarted. Will load all of the triangles at once and hide the
	 * ones that aren't within 3 away of a shot behind the queue. This method must be called when a 
	 * level is beaten or restarted. It will call level parser to get the level information for the queue
	 */
	public void loadLevel() {
		LevelParser parser = GameObject.FindGameObjectWithTag ("GameManager").GetComponentInChildren<LevelParser>(); 
		string [] queueTris = parser.getQueueArray ();

		inQueue = new GameObject[queueTris.Length];

		GameObject triInstance = null;
		
		float yLoc = 0.0f;
		for(int i = 0; i < inQueue.Length; i++) {
			queueTris[i] = queueTris[i].Trim();
			if(queueTris[i].Equals(RED)) {
				triInstance = (GameObject) Instantiate(redQueueTriangle);
			}
			else if(queueTris[i].Equals(GREEN)) {
				triInstance = (GameObject) Instantiate(greenQueueTriangle);
			}
			else if(queueTris[i].Equals(BLUE)) {
				triInstance = (GameObject) Instantiate(blueQueueTriangle);
			}
			else if(queueTris[i].Equals(AQUA)) {
				triInstance = (GameObject) Instantiate(aquaQueueTriangle);
			}
			else if(queueTris[i].Equals(PINK)) {
				triInstance = (GameObject) Instantiate(pinkQueueTriangle);
			}
			else {
				triInstance = (GameObject) Instantiate(yellowQueueTriangle);
			}

			if (i < 3) {
				yLoc += distance;
				triInstance.transform.localPosition = new Vector3(6.0f, yLoc, -1.5f);
			}
			else {
				triInstance.transform.localPosition = new Vector3(6.0f, yLoc, 1.0f);
			}
			
			triInstance.transform.localRotation = new Quaternion(0,0,0,0);
			triInstance.transform.localScale = new Vector3(1,1,1);
			inQueue[i] = triInstance;
		}
	}

	private string GetColourFromName(string name)
	{
		if(name.Contains("Aqua"))
		{
			return AQUA;
		}
		else if(name.Contains("Blue"))
		{
			return BLUE;
		}
		else if(name.Contains("Green"))
		{
			return GREEN;
		}
		else if(name.Contains("Pink"))
		{
			return PINK;
		}
		else if(name.Contains("Red"))
		{
			return RED;
		}
		else if(name.Contains("Yellow"))
		{
			return YELLOW;
		}
		else
		{
			return BLUE;
		}

	}
}
