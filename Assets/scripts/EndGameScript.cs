using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {

	public GUISkin skin;//skin for buttons and backdrop
	Rect backWindow; //button's backdrop
	Rect label;

	
	private void Start(){
		//back drop dimensions
		float backW = 3 * Screen.width/4;
		float backH = Screen.height/2;
		//center the back drop
		float halfScreenW = (Screen.width/2) - backW/2;
		float halfScreenH = (Screen.height/2) - backH/2;
		
		backWindow = new Rect(halfScreenW, halfScreenH, backW, backH);//build backdrop

		//set the label dimensions for the 
		backW = (Screen.width/2) - backW/2;
		backH = (Screen.height/2) - backH/2;

		halfScreenW = (Screen.width/2) - backW/2;
		halfScreenH = backH + Screen.height/11;

		label = new Rect(0, halfScreenH, Screen.width, halfScreenH);
	}
	
	void Update(){

	}
	
	void OnGUI(){
	
		//button dimensions
		float buttonW = Screen.width/2;
		float buttonH = Screen.height/10;
		
		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 2.5f);
		
		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.fontSize = (int)(buttonH / 2.5f);
		labelStyle.alignment = TextAnchor.UpperCenter;
		
		GUI.skin = skin;
		style.alignment = TextAnchor.UpperCenter;
		GUI.Box(backWindow, "Game Over!", style);//title the menu
		style.alignment = TextAnchor.MiddleCenter;
		
		GUI.Label(label, "Score: " + GlobalFlags.getScore(), labelStyle);
		
		// set the resume button and functionality
		float halfScreenW = (Screen.width/2) - buttonW/2;
		float halfScreenH = Screen.height/2 - 80;
		/*if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Resume")){
			paused = false;
			GlobalFlags.setPaused(false);
		}*/
		
		//set the restart button and functionality
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = backWindow.y + Screen.height/5;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Restart", style)){
			//GlobalFlags.canFire = true;
			Application.LoadLevel("game");
			GlobalFlags.setPaused(false);
		}
		
		//set the back to menu button and functionality
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = Screen.height/2 + buttonH * 1.2f;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Menu", style)){
			//GlobalFlags.canFire = true;
			Application.LoadLevel("LevelSelectPreMenu");
			GlobalFlags.setPaused(false);
		}
	}
	
	void windowFunc(int id){
		
	}
}
