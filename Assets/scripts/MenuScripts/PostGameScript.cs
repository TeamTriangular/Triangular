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

		if (Resources.Load("levels/level" + (GlobalFlags.getLevel() + 1))){
			//System.IO.File.ReadAllLines("assets/levels/level" + (GlobalFlags.getLevel() + 1) + ".txt");
			//GlobalFlags.setLevel(GlobalFlags.getLevel() + 1);

			float buttonW = Screen.width/2.8f;
			float buttonH = Screen.width/6;
			
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fontSize = (int)(buttonH / 2.8);
			
			//set the play game button
			float halfScreenW = (Screen.width/4) - buttonW/2;
			float halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Replay", style)){
				Application.LoadLevel("game");
			}

			halfScreenW = (3 * Screen.width/4) - buttonW/2;
			halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Play Next", style)){
				GlobalFlags.setLevel(GlobalFlags.getLevel() + 1);
				Application.LoadLevel("game");
			}
			
			//set the exit button
			halfScreenW = (Screen.width/4) - buttonW/2;
			halfScreenH = Screen.height/2 + buttonH * 1.5f;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Level Select", style)){
				Application.LoadLevel("LevelSelectPreMenu");
			}
			
			//set the exit button
			halfScreenW = (3 * Screen.width/4) - buttonW/2;
			halfScreenH = Screen.height/2 + buttonH * 1.5f;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Main Menu", style)){
				Application.LoadLevel("ModeSelectMenu");
			}

		}
		else{
			float buttonW = Screen.width/2.8f;
			float buttonH = Screen.width/6;
			
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fontSize = (int)(buttonH / 2.8);
			
			//set the play game button
			float halfScreenW = (Screen.width/4) - buttonW/2;
			float halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Replay", style)){
				Application.LoadLevel("game");
			}

			halfScreenW = (3 * Screen.width/4) - buttonW/2;
			halfScreenH = (Screen.height/2) + 0;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Main Menu", style)){
				Application.LoadLevel("ModeSelectMenu");
			}
		}
	}
}
