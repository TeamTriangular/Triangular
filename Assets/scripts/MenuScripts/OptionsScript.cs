using UnityEngine;
using System.Collections;

public class OptionsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = 100;
		float buttonH = 50;
		float halfScreenW, halfScreenH; 
		//set the prebuilt button
		
		//set the back button
		halfScreenW = (Screen.width/2) - 50;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back")){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
