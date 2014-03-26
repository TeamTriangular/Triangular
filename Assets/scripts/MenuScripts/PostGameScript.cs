using UnityEngine;
using System.Collections;

public class PostGameScript : MonoBehaviour {

	public GUIText title;
	public GUIText score;

	// Use this for initialization
	void Start () {
		score.text = "Score: " + GlobalFlags.getScore();
		title.text = "Level: " + GlobalFlags.getLevel();
	}
	
	void OnGUI () {
		//set demensions of the buttons

		if (System.IO.File.Exists("assets/levels/level" + (GlobalFlags.getLevel() + 1) + ".txt")){
			//System.IO.File.ReadAllLines("assets/levels/level" + (GlobalFlags.getLevel() + 1) + ".txt");
			//GlobalFlags.setLevel(GlobalFlags.getLevel() + 1);

			float buttonW = 150;
			float buttonH = 50;
			
			//set the play game button
			float halfScreenW = (Screen.width/2) - 200;
			float halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Replay")){
				Application.LoadLevel("game");
			}

			halfScreenW = (Screen.width/2) +50;
			halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Play Next")){
				GlobalFlags.setLevel(GlobalFlags.getLevel() + 1);
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
		else{
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
}
