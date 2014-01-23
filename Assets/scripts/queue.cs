using UnityEngine;
using System.Collections;

public class queue : MonoBehaviour {

	GameObject [] inQueue;

	public GameObject [] level1;

	public GameObject [] level2;

	public GameObject [] level3;

	public GameObject [] level4;

	public GameObject [] level5;

	public GameObject trianglePrefab;

	//distance between each triangle in the queue
	private float distance = 1.5f;

	private int currentLevel;

	private int currentTri = 0;
	
	/**
	 * Used to load level 1 into the queue. All further levels will be checked with the update script. 
	 */
	void Start () {
		currentLevel = 1;
		loadLevel (currentLevel);
	}

	/**
	 * Called by the generator whenever a shot is fired to make sure that it can fire a shot
	 * And to let the queue know that it has to update. Will return the triangle that is to be shot,
	 * otherwise it will return null
	 */
	public GameObject fireShot() {
		if(currentTri < inQueue.Length) {
			GameObject returnTri = (GameObject) Instantiate (trianglePrefab);
			returnTri.renderer.material = inQueue[currentTri].renderer.material;

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
	 * level is beaten or restarted. 
	 */
	public void loadLevel(int num) {
		if(num == 1) {
			inQueue = new GameObject[level1.Length];
		}
		else if(num == 2) {
			inQueue = new GameObject[level2.Length];
		}
		else if(num == 3) {
			inQueue = new GameObject[level3.Length];
		}
		else if(num == 4) {
			inQueue = new GameObject[level4.Length];
		}
		else if(num == 5) {
			inQueue = new GameObject[level5.Length];
		}

		GameObject triInstance = null;
		
		float yLoc = 0.0f;
		for(int i = 0; i < inQueue.Length; i++) {
			if (i < 3) {
				yLoc += distance;

				if(num == 1) {
					triInstance = (GameObject) Instantiate(level1[i]);
				}
				else if(num == 2) {
					triInstance = (GameObject) Instantiate(level2[i]);
				}
				else if(num == 3) {
					triInstance = (GameObject) Instantiate(level3[i]);
				}
				else if(num == 4) {
					triInstance = (GameObject) Instantiate(level4[i]);
				}
				else if(num == 5) {
					triInstance = (GameObject) Instantiate(level5[i]);
				}

				triInstance.transform.localPosition = new Vector3(6.0f, yLoc, -1.5f);
			}
			else {
				triInstance = (GameObject) Instantiate(level1[i]);
				triInstance.transform.localPosition = new Vector3(6.0f, yLoc, 1.0f);
			}
			
			triInstance.transform.localRotation = new Quaternion(0,0,0,0);
			triInstance.transform.localScale = new Vector3(1,1,1);
			inQueue[i] = triInstance;
		}
	}
	
}
