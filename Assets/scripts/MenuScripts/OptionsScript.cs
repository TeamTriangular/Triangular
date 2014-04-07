using UnityEngine;
using System.Collections;

public class OptionsScript : MonoBehaviour {

	float musicVolume, effectVolume;

	// Use this for initialization
	void Start () {
		musicVolume = GlobalFlags.getMusicVolume();
		effectVolume = GlobalFlags.getEffectVolume();
	}
	
	void OnGUI () {
		//set demensions of the buttons
		float buttonW = Screen.width/3;
		float buttonH = Screen.height/15;
		float halfScreenW, halfScreenH; 
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = (int)(buttonH/2);

		//music volume slider
		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/6 + (buttonH * 1.5f) * 1;
		GUI.Label(new Rect(halfScreenW,halfScreenH,buttonW,buttonH), "Music", style);

		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/6 + (buttonH * 1.5f) * 2;
		musicVolume = GUI.HorizontalSlider(new Rect(halfScreenW,halfScreenH,buttonW,buttonH), musicVolume, 0.0f, 1.0f);
		GlobalFlags.setMusicVolume(musicVolume);
		GameObject.Find ("Music").GetComponent<Music>().UpdateVolume();

		//sound effects volume slider
		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/6 + (buttonH * 1.5f) * 3;
		GUI.Label(new Rect(halfScreenW,halfScreenH,buttonW,buttonH), "Sound FX", style);

		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/6 + (buttonH * 1.5f) * 4;
		effectVolume = GUI.HorizontalSlider(new Rect(halfScreenW,halfScreenH,buttonW,buttonH), effectVolume, 0.0f, 1.0f);
		GlobalFlags.setEffectVolume(effectVolume);
		GameObject.Find ("SoundEffects").GetComponent<SoundEffects>().UpdateVolume();
		
		//set the back button
		style = new GUIStyle(GUI.skin.button);
		style.fontSize = (int)(buttonH / 2);
		halfScreenW = Screen.width/2 - buttonW/2;
		halfScreenH = Screen.height/1.1f - buttonH/2;
		if (GUI.Button(new Rect(halfScreenW,halfScreenH,buttonW,buttonH),"Back", style)){
			Application.LoadLevel("ModeSelectMenu");
		}
	}
	
}
