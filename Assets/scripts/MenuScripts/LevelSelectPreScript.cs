using UnityEngine;
using System.Collections;

public class LevelSelectPreScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = Screen.width/3;
		float buttonH = Screen.height/16;
		float halfScreenW, halfScreenH; 
		//set the prebuilt button
		
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 2);

		// -430 -290 -170 -50 70 190 310
		for (int i = 0; i < 7; i++)
		{
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/6 + (buttonH * 1.5f) * i;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Level: "+(i+1), style)){
				GlobalFlags.setLevel(i+1);
				Application.LoadLevel("game");
			}
		}
		
		//set the back button
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = Screen.height/6 + (buttonH * 1.5f) * 7;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back", style)){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
