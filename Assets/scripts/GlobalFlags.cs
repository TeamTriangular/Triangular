using UnityEngine;
using System.Collections;

/**
 * Global variables that the game needs to be able to access from anywhere
 */
public class GlobalFlags : MonoBehaviour {

	public static bool canFire = true;

	public static int level = 1; 

	/*
	 * Set for if you want the level to be randomly generated or not
	 */
	public static bool isRandLevel = true;

	/*
	 * Called when the user selects they want to play random levels
	 */
	public static void setRandLevel(bool rLevel) {
		isRandLevel = rLevel;
	}

	/*
	 * Used by the level parser to know if it should be parsing a level that's random, or a specific one. 
	 */
	public static bool getRandLevel() {
		return isRandLevel;
	}

	/**
	 * Made so that when the level is beaten any script can call this and change it to the next level
	 */
	public static void setLevel(int num) {
		level = num;
	}

	/*
	 * For any script that needs to know the current level the user is on
	 */
	public static int getLevel() {
		return level;
	}
}
