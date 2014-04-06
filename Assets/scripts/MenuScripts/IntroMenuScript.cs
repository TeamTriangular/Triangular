using UnityEngine;
using System.Collections;

public class IntroMenuScript : MonoBehaviour {

	void Awake()
	{
	
	}
	
	void Update () { 
		//transform.Rotate(new Vector3(0.0f, rotationAmount, 0.0f)); //rotate the camera arround the map
		//this scene is now just used to create music game objects, we load next scene right away
		Application.LoadLevel("ModeSelectMenu");
	}
	
//	void OnGUI()
//	{
//		
//		//set demensions of the buttons
//		float buttonW = 2 * Screen.width/3;
//		float buttonH = Screen.height/8;
//		
//		//set the play game button
//		float halfScreenW = (Screen.width/2) - buttonW/2;
//		float halfScreenH = Screen.height/2 - Screen.height/10;
//		
//		GUIStyle style = new GUIStyle(GUI.skin.button);
//		style.fontSize = (int)(buttonH / 2);
//
//		
//		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Play Game", style)){
//			Application.LoadLevel("ModeSelectMenu");
//		}
//		
//		//set the exit button
//		halfScreenW = (Screen.width/2) - buttonW/2;
//		halfScreenH = Screen.height/2 + buttonH * 1.5f;
//		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Exit", style)){
//			Application.Quit();
//		}
//	}
}
