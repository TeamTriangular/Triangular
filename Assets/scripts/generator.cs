using UnityEngine;
using System.Collections;

public class generator : MonoBehaviour {

	public GameObject prefab;
	public GameObject shooter;
	public float shootingForce;
	
	// Update is called once per frame
	void Update () 
	{
 		GameObject cannon = GameObject.FindGameObjectWithTag("Player");
		
		if(Input.GetMouseButtonUp(0) && GlobalFlags.canFire)
		{
			GameObject instance = Instantiate(prefab, shooter.transform.position + 
				((shooter.transform.rotation * Vector3.up) * 0.3f), shooter.transform.rotation) as GameObject;
			instance.rigidbody.AddForce((shooter.transform.rotation * Vector3.up) * shootingForce);
			
			cannon.transform.localRotation = Quaternion.Euler(new Vector3(0,0,180));
			
			GlobalFlags.canFire = false;
		}
	
	}
}
