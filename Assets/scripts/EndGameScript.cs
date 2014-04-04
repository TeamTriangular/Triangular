using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {

	public GUISkin skin;//skin for buttons and backdrop
	Rect backWindow; //button's backdrop
	Rect label;
	
	private void Start(){
		//back drop dimensions
		float backW = 200;
		float backH = 250;
		//center the back drop
		float halfScreenW = (Screen.width/2) - backW/2;
		float halfScreenH = (Screen.height/2) - backH/2;
		
		backWindow = new Rect(halfScreenW, halfScreenH, backW, backH);//build backdrop


		//set the label dimensions for the 
		backW = 50;
		backH = 150;

		halfScreenW = (Screen.width/2) - backW/2;
		halfScreenH = (Screen.height/2) - backH/2;

		label = new Rect(halfScreenW, halfScreenH, 100, 50);
	}
	
	void Update(){

	}
	
	void OnGUI(){

		GUI.skin = skin;
		GUI.Box(backWindow, "Game Over!");//title the menu
		
		//button dimensions
		float buttonW = 100;
		float buttonH = 50;

		string score = "Score: ";

		GUI.Label(label, "Score: " + GlobalFlags.getScore());
		
		// set the resume button and functionality
		float halfScreenW = (Screen.width/2) - buttonW/2;
		float halfScreenH = Screen.height/2 - 80;
		/*if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Resume")){
			paused = false;
			GlobalFlags.setPaused(false);
		}*/
		
		//set the restart button and functionality
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = Screen.height/2 - 20;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Restart")){
			//GlobalFlags.canFire = true;
			Application.LoadLevel("game");
			GlobalFlags.setPaused(false);
		}
		
		//set the back to menu button and functionality
		halfScreenW = (Screen.width/2) - buttonW/2;
		halfScreenH = Screen.height/2 + 40;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back To Menu")){
			//GlobalFlags.canFire = true;
			Application.LoadLevel("LevelSelectPreMenu");
			GlobalFlags.setPaused(false);
		}
	}
	
	void windowFunc(int id){
		
	}
}
