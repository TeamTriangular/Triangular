using UnityEngine;
using System.Collections;

public class LevelSelectUserScript : MonoBehaviour {

	/*********************************************************
	 * Application Loads for this script need to be completed
	 * once those game types begin implementation.
	 */ 

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = 100;
		float buttonH = 50;
		
		//set the play user button
		float halfScreenW = (Screen.width/2) - 185;
		float halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW+30,buttonH),"Play User Made")){
			Application.LoadLevel("game");
		}
		
		//set the build your own button
		halfScreenW = (Screen.width/2) + 55;
		halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW+30,buttonH),"Build Your Own")){
			Application.LoadLevel("game");
		}
		
		//set the back button
		halfScreenW = (Screen.width/2) - 50;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back")){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
