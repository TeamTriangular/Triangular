using UnityEngine;
using System.Collections;

public class OptionsScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = Screen.width/3;
		float buttonH = Screen.height/15;
		float halfScreenW, halfScreenH; 
		
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 2);

		//set the prebuilt button
		
		//set the back button
		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/2 - buttonH/2;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back", style)){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
