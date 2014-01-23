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

	/// <summary>
	/// Sets the colour and material of the triangle based on
    /// the color of the material given
	/// 
	/// Defaults to blue
	/// </summary>
	/// <param name="m">Material to base the color on</param>
	public void SetColourFromMaterial (Material m)
	{
		Color c = m.color;

		if(c == aqua.color)
		{
			colour = Color.cyan;
		}
		else if(c == black.color)
		{
			colour = Color.black;
		}
		else if(c == blue.color)
		{
			colour = Color.blue;
		}
		else if(c == green.color)
		{
			colour = Color.green;
		}
		else if(c == red.color)
		{
			colour = Color.red;
		}
		else if(c == yellow.color)
		{
			colour = Color.yellow;
		}
		else if(c == pink.color)
		{
			colour = Color.magenta;
		}
		else
		{
			Debug.Log("defaulting to blue");
			colour = Color.blue;
		}

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
			Debug.Log("defaulting to blue");
			renderer.material = blue;
		}
	}
}
