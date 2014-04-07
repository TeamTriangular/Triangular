using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	public GUISkin skin;//skin for buttons and backdrop
	Rect backWindow; //button's backdrop
	bool paused = false;//paused state, true the menu is open, false closed
	
	private void Start(){
		//back drop dimensions
		float backW = 3 * Screen.width/4;
		float backH = Screen.height/2;
		//center the back drop
		float halfScreenW = (Screen.width/2) - backW/2;
		float halfScreenH = (Screen.height/2) - backH/2;
		
		backWindow = new Rect(halfScreenW, halfScreenH, backW, backH);//build backdrop

	}
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)){ 
			//if escape is pressed open/close the menu, 
			if (paused){ 
				//if open, then close the menu
				paused = false;
				GlobalFlags.setPaused(false);
			}
			else{
				//if closed, open the menu
				paused = true;
				GlobalFlags.setPaused(true);
			}
		}
	}
	
	void OnGUI(){

		//button dimensions
		float buttonW = Screen.width/4.8f;;
		float buttonH = Screen.height/6;

		float halfScreenW = (Screen.width) - (buttonW*1.1f);
		//float halfScreenH = Screen.height/13;
		float halfScreenH = buttonH/3;

		GUIStyle style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 3.5f);
		
		GUI.skin = skin;

		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"||", style)){
			paused = true;
			GlobalFlags.setPaused(true);
		}
		
		if (paused){
			//if paused build the menu

			buttonW = Screen.width/2;
			buttonH = Screen.height/10;

			style.alignment = TextAnchor.UpperCenter;
			GUI.Box(backWindow, "Pause Menu", style);//title the menu
			style.alignment = TextAnchor.MiddleCenter;
			
			// set the resume button and functionality
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/3;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Resume", style)){
				paused = false;
				GlobalFlags.setPaused(false);
			}
			
			//set the restart button and functionality
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/3 + buttonH * 1.5f;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Restart", style)){
				//GlobalFlags.canFire = true;
				Application.LoadLevel("game");
				GlobalFlags.setPaused(false);
			}
			
			//set the back to menu button and functionality
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/3 + buttonH * 3;
			if (!GlobalFlags.getRandLevel()) {
				if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Menu", style)){
					//GlobalFlags.canFire = true;
					Application.LoadLevel("LevelSelectPreMenu");
					GlobalFlags.setPaused(false);
				}
			}
			else{
				if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Menu", style)){
					//GlobalFlags.canFire = true;
					GlobalFlags.setRandLevel(false);
					GlobalFlags.infiniteRandomMode = false; 
					Application.LoadLevel("ModeSelectMenu");
					GlobalFlags.setPaused(false);
				}
			}
		}
	}
	
	void windowFunc(int id){
		
	}
}
