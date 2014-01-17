using UnityEngine;
using System.Collections;

public class PostGameScript : MonoBehaviour {

	public GUIText title;
	public GUIText score;

	// Use this for initialization
	void Start () {
		score.text = "Score: " + 150000;
		title.text = "Level: " + 1;
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = 100;
		float buttonH = 50;
		
		//set the play game button
		float halfScreenW = (Screen.width/2) - 50;
		float halfScreenH = Screen.height/2 + 0;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Replay")){
			Application.LoadLevel("game");
		}

		buttonW = 150;

		//set the exit button
		halfScreenW = (Screen.width/2) - 200;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Level Select")){
			Application.LoadLevel("LevelSelectPreMenu");
		}

		//set the exit button
		halfScreenW = (Screen.width/2) + 50;
		halfScreenH = Screen.height/2 + 60;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Main Menu")){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
}
