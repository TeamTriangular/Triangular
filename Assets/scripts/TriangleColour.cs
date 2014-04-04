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
	
	public static readonly string RED = "r";
	public static readonly string GREEN = "g";
	public static readonly string BLUE = "b";
	public static readonly string AQUA = "a";
	public static readonly string PINK = "p";
	public static readonly string YELLOW = "y";

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

	public void SetColourFromString(string s)
	{
		if(s.Equals(AQUA))
		{
			SetColour(Color.cyan);
		}
		else if(s.Equals(BLUE))
		{
			SetColour(Color.blue);
		}
		else if(s.Equals(GREEN))
		{
			SetColour(Color.green);
		}
		else if(s.Equals(RED))
		{		
			SetColour(Color.red);
		}
		else if(s.Equals(YELLOW))
		{
			SetColour(Color.yellow);
		}
		else if(s.Equals(PINK))
		{
			SetColour(Color.magenta);
		}
		else
		{
			//Defaulting on blue
			SetColour(Color.blue);
		}
	}

	public string GetCurrentColourAsString()
	{
		if(colour == Color.cyan)
		{
			return AQUA;
		}
		if(colour == Color.blue)
		{
			return BLUE;
		}
		if(colour == Color.green)
		{
			return GREEN;
		}
		if(colour == Color.red)
		{
			return RED;
		}
		if(colour == Color.yellow)
		{
			return YELLOW;
		}
		if(colour == Color.magenta)
		{
			return PINK;
		}
		else
		{
			//Defaulting on blue
			return BLUE;
		}
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
