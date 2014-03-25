using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	public GUISkin skin;//skin for buttons and backdrop
	Rect backWindow; //button's backdrop
	bool paused = false;//paused state, true the menu is open, false closed
	
	private void Start(){
		//back drop dimensions
		float backW = 200;
		float backH = 250;
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
				GlobalFlags.canFire = true;
			}
			else{
				//if closed, open the menu
				paused = true;
				GlobalFlags.canFire = false;
			}
		}
	}
	
	void OnGUI(){
		
		if (paused){
			//if paused build the menu
			GUI.skin = skin;
			GUI.Box(backWindow, "Pause Menu");//title the menu
			
			//button dimensions
			float buttonW = 100;
			float buttonH = 50;
			
			// set the resume button and functionality
			float halfScreenW = (Screen.width/2) - buttonW/2;
			float halfScreenH = Screen.height/2 - 80;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Resume")){
				paused = false;
			}
			
			//set the restart button and functionality
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/2 - 20;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Restart")){
				//GlobalFlags.canFire = true;
				Application.LoadLevel("game");
			}
			
			//set the back to menu button and functionality
			halfScreenW = (Screen.width/2) - buttonW/2;
			halfScreenH = Screen.height/2 + 40;
			if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Menu")){
				//GlobalFlags.canFire = true;
				Application.LoadLevel("LevelSelectPreMenu");
			}
		}
	}
	
	void windowFunc(int id){
		
	}
}
