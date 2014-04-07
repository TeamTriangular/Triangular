using UnityEngine;
using System.Collections;

public class SoundEffects : MonoBehaviour {

	public AudioClip pterodactyl;

	public AudioClip machoMadness;

	public AudioClip gameOver;

	void Start() {
		audio.volume = GlobalFlags.getEffectVolume();
	}

	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

	public void setSoundEffectClip() {

	}

	public void playSoundEffect(string choice) {
		if(string.Compare(choice, "pterodactyl") == 0){
			audio.clip = pterodactyl;
		}
		else if(string.Compare(choice, "machoMadness") == 0) {
			audio.clip = machoMadness;
		}
		else if(string.Compare(choice, "gameOver") == 0) {
			audio.clip = gameOver;
		}
		audio.Play ();
	}

	public void UpdateVolume()
	{
		audio.volume = GlobalFlags.getEffectVolume();
	}
}
