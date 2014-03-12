using UnityEngine;
using System.Collections;

public class ModeSelectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = 100;
		float buttonH = 50;
		
		//set the prebuilt button
		float halfScreenW = (Screen.width/2) - 170;
		float halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Original")){
			Application.LoadLevel("LevelSelectPreMenu");
		}

		//set the random button
		halfScreenW = (Screen.width/2) - 50;
		halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Random")){
			GlobalFlags.setRandLevel (true);
			Application.LoadLevel("game");
		}

		//set the user made button
		halfScreenW = (Screen.width/2) + 70;
		halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"User Made")){
			Application.LoadLevel("LevelSelectUserMenu");
		}
		
		//set the option button
		halfScreenW = (Screen.width/2) - 50;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Options")){
			Application.LoadLevel("OptionsMenu");
		}
	}

}
