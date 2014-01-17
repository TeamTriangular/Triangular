using UnityEngine;
using System.Collections;

public class LevelSelectPreScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = 100;
		float buttonH = 50;
		float halfScreenW, halfScreenH; 
		//set the prebuilt button

		// -430 -290 -170 -50 70 190 310
		for (int i = 0; i < 7; i++){
			halfScreenW = (Screen.width/2) - 410 + (i*120);
			halfScreenH = Screen.height/2 + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Level: "+(i+1))){
				Application.LoadLevel("game");
			}
		}
		
		//set the back button
		halfScreenW = (Screen.width/2) - 50;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back")){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
