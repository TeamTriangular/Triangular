using UnityEngine;
using System.Collections;

public class TriangleColour : MonoBehaviour {

	public Color colour = new Color();
	public Material aqua;
	public Material black;
	public Material blue;
	public Material green;
	public Material pink;
	public Material red;
	public Material yellow;

	//This is for testing...allows the materials to be changed on the fly
	/*
	void Update()
	{
		SetMaterial();
	}
	*/

	public Color GetColour ()
	{
		return colour;
	}

	public void SetColour (Color c)
	{
		colour = c;
		SetMaterial();
	}

	private void SetMaterial()
	{
		if(colour == Color.cyan)
		{
			renderer.material = aqua;
		}
		else if(colour == Color.black)
		{
			renderer.material = black;
		}
		else if(colour == Color.blue)
		{
			renderer.material = blue;
		}
		else if(colour == Color.green)
		{
			renderer.material = green;
		}
		else if(colour == Color.red)
		{
			renderer.material = red;
		}
		else if(colour == Color.yellow)
		{
			renderer.material = yellow;
		}
		else if(colour == Color.magenta)
		{
			renderer.material = pink;
		}
		else
		{
			renderer.material = blue;
		}
	}
}
