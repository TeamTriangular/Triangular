using UnityEngine;
using System.Collections;

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

	//distance between each triangle in the queue
	private float distance = 1.5f;

	private int currentTri = 0;
	
	/**
	 * Used to load level 1 into the queue. All further levels will be checked with the update script. 
	 */
	void Start () {
		loadLevel ();
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
	
}
