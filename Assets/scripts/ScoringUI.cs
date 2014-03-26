using UnityEngine;
using System.Collections;

public class ScoringUI : MonoBehaviour {

	void OnGUI() 
	{
		GUI.Label(new Rect(0,0,500,25),"Score: " +  GlobalFlags.getScore());
		GUI.Label(new Rect(0,25,500,25),"Multiplier: " +  GlobalFlags.getMultiplier());
		GUI.Label(new Rect(0,50,500,100),"Queue Bonus: " +  GlobalFlags.getQueueBounusTotal());
	}
}
