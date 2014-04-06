using UnityEngine;
using System.Collections;

public class ModeSelectScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = Screen.width/3;
		float buttonH = Screen.height/15;
		
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 2);
		
		//set the prebuilt button
		float halfScreenW = (Screen.width/2) - buttonW/2;
		float halfScreenH = 5 * Screen.height/12;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Original", style)){
			Application.LoadLevel("LevelSelectPreMenu");
		}

		//set the random button
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = 6 * Screen.height/12;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Random", style)){
			GlobalFlags.setRandLevel (true);
			Application.LoadLevel("game");
		}

		//set the user made button
//		halfScreenW = (Screen.width/2) - buttonW/2;
//		halfScreenH = 7 * Screen.height/12 + 0;
//		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"User Made", style)){
//			Application.LoadLevel("LevelSelectUserMenu");
//		}
		
		//set the option button
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = 10 * Screen.height/12 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Options", style)){
			Application.LoadLevel("OptionsMenu");
		}
	}

}
