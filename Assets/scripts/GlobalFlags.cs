using UnityEngine;
using System.Collections;

/**
 * Global variables that the game needs to be able to access from anywhere
 */
public class GlobalFlags : MonoBehaviour {

	public static bool canFire = true;

	public static int level = 1; 

	/**
	 * Made so that when the level is beaten any script can call this and change it to the next level
	 */
	public void setLevel(int num) {
		level = num;
	}

	/*
	 * For any script that needs to know the current level the user is on
	 */
	public static int getLevel() {
		return level;
	}
}
