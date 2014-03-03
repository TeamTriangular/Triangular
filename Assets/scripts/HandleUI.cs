using UnityEngine;
using System.Collections;

public class HandleUI : MonoBehaviour {

	public Texture UIAqua;
	public Texture UIBlue;
	public Texture UIGreen;
	public Texture UIPink;
	public Texture UIRed;
	public Texture UIYellow;
	public Texture UIDelete;

	private int currentColour;
	private string[] colours = {"a","b","g","p","r","y"};
	private Texture[] textures;
	private bool adding;
	private bool saving;
	private int savingTime;
	private string levelName = "LevelName";

	// Use this for initialization
	void Start () 
	{
		textures = new Texture[6];
		textures[0] = UIAqua;
		textures[1] = UIBlue;
		textures[2] = UIGreen;
		textures[3] = UIPink;
		textures[4] = UIRed;
		textures[5] = UIYellow;
		currentColour = 0;
		adding = true;
		saving = false;
		savingTime=0;
	}

	void Update()
	{
		if(saving)
		{
			if(savingTime < 100)
			{
				savingTime++;
			}
			else
			{
				savingTime = 0;
				saving = false;
			}
		}
	}

	void OnGUI() 
	{
		if(adding)
		{
			GUI.DrawTexture(new Rect(10, 0, 80, 80), textures[currentColour]);

			if(GUI.Button(new Rect(0, 85, 40, 20), "<--"))
			{
				currentColour--;
				if(currentColour < 0)
				{
					currentColour = textures.Length - 1;
				}
			}

			if(GUI.Button(new Rect(60, 85, 40, 20), "-->"))
			{
				currentColour++;
				if(currentColour >= textures.Length)
				{
					currentColour = 0;
				}
			}
			
			if(GUI.Button(new Rect(60, 110, 40, 20), "Del"))
			{
				adding = false;	
			}
		}
		else
		{
			GUI.DrawTexture(new Rect(10, 0, 80, 80), UIDelete);

			if(GUI.Button(new Rect(0, 110, 40, 20), "Add"))
			{
				adding = true;
			}
		}

		GameObject q = GameObject.FindGameObjectWithTag("Queue");
		string s = "Queue Size: " + q.GetComponent<queue>().GetQueueSize().ToString();
		GUI.Label(new Rect(0, 135, 100, 20),s); 

		levelName = GUI.TextField(new Rect(0,160,100,20),levelName);
		if(GUI.Button (new Rect(0,185,100,20), "Save"))
		{
			string[] qString = q.GetComponent<queue>().toStringArray();
			string[,] grid = GetComponent<TriangleGrid>().toStringArray();
			GetComponent<LevelParser>().CreateLevelFile(levelName,qString,grid);
			saving = true;
		}
		if(GUI.Button (new Rect(0,210,100,20),"Exit"))
		{
			Application.LoadLevel("LevelSelectUserMenu");
		}
		if(saving)
		{
			GUI.Label (new Rect(0,235,100,20),"Saving...");
		}
	}

	public string GetColourString()
	{
		return colours[currentColour];
	}

	public bool IsAdding()
	{
		return adding;
	}
}
