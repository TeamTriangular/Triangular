using UnityEngine;
using System.Collections;

/**
 * Level parser will parse a level file, depending on what is passed to it during construction
 * And contrains methods for gathering information from the file to help build levels,
 * whether it be needed for the level itself or the queue that is needed to beat the level. 
 *
 * Note about level file: The first line is the first letter of the triangles in the queue. B = blue, G = Green
 * R = red, etc. The next lines are all locations of the triangles on the grid itself, followed by what colour that 
 * triangle is. 
 */
public class LevelParser : MonoBehaviour {

	/**
	 * Represents the triangle of a given line in the level file. Will also contain information about what is next in the queue.
	 */
	public struct TriInfo {
		int x;
		int y;
		string colour;

		public TriInfo(int x, int y, string colour) {
			this.x = x;
			this.y = y;
			this.colour = colour;
		}

		public void setX(int xVal) {
			x = xVal;
		}
		public int getX() {
			return x;
		}
		public void setY(int yVal) {
			y = yVal;
		}
		public int getY() {
			return y;
		}
		public void setColour(string colourVal) {
			colour = colourVal;
		}
		public string getColour() {
			return colour;
		}
	}

	private TriInfo [] triArray;

	private string [] queueTris;

	/**
	 * This has to be awake, as the levels have to be parsed before everything else calls the data
	 * from the file. 
	 */
	void Awake() {
		//TODO Once we have levels being set globally, get the level here from the global variables. 
		try {

			string[] lines = System.IO.File.ReadAllLines("assets/levels/level" + 1  + ".txt");

			triArray = new TriInfo[lines.Length - 1]; //first line of the level file is the queue for that level
			queueTris = lines[0].Split(',');
			for(int i=1; i< lines.Length; i++) {
				int commaIndex = lines[i].IndexOf(',');
				int colonIndex = lines[i].IndexOf(':');
				int x = int.Parse(lines[i].Substring(0, commaIndex));
				int y = int.Parse(lines[i].Substring(commaIndex + 1, colonIndex - 1 - commaIndex));
				string colour = lines[i].Substring(colonIndex + 1, lines[i].Length - 1 - colonIndex);

				TriInfo triInfo = new TriInfo(x, y, colour);

				triArray[i - 1] = triInfo; // first "i" was for the queue
			}
		}
		catch {
			Debug.Log("error reading file level" + 1 + ".txt");
		}
	}

	/**
	 * Gets the array that contains the colour of each triangle and the location in x y coordinates 
	 */
	public TriInfo [] getTriArray() {
		return triArray;
	}

	/**
	 * Gets the array containing all of the colours for the queue, represented by letters
	 */
	public string [] getQueueArray() {
		return queueTris;
	}

}
