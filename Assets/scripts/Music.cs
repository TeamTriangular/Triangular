using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private float seStartTime = 0.0f;

	private float lengthOfEffect = 0.0f;

	SoundEffects se;

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindGameObjectWithTag ("SoundEffect");
		se = go.GetComponent<SoundEffects>();
	}
	
	// Update is called once per frame
	void Update () {
		if(audio != null && !isSoundEffectPlaying(Time.time)) {
			this.audio.volume = 1.0f;
		}
	}

	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

	public bool isSoundEffectPlaying(float currentTime) {
		return (currentTime - seStartTime) < lengthOfEffect;
	}

	public void setSeStartTime(float start, float length) {
		seStartTime = start;
		lengthOfEffect = length;
	}

	public void playSoundEffect(string choice) {
		se.playSoundEffect (choice);
	}

}
