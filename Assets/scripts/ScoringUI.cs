using UnityEngine;
using System.Collections;

public class ScoringUI : MonoBehaviour {

	void OnGUI() 
	{
		int height = Screen.height/15;
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = (int)(height/2);
		
		GUI.Label(new Rect(0,0,Screen.width,height),"Score: " +  GlobalFlags.getScore(), style);
		GUI.Label(new Rect(0,1 * Screen.height/15,Screen.width,height),"Multiplier: " +  GlobalFlags.getMultiplier(), style);
		if (!GlobalFlags.infiniteRandomMode){ 
			GUI.Label(new Rect(0,2 * Screen.height/15,Screen.width,height),"Queue Bonus: " +  GlobalFlags.getQueueBounusTotal(), style);
		}
	}
}
