using UnityEngine;
using System.Collections;

/**
 * For shooting the triangles. Sets a bool so that only one triangle can be fired at once,
 * Applys the necessary forces of the triangle.
 */
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
				((shooter.transform.rotation * Vector3.up) * 0.3f), Quaternion.identity) as GameObject;
			instance.rigidbody.AddForce((shooter.transform.rotation * Vector3.up) * shootingForce);
			
			Vector3 rot = transform.rotation.eulerAngles;
			rot.z -= 60;
			instance.transform.rotation = Quaternion.Euler(rot);
			
			cannon.transform.localRotation = Quaternion.Euler(new Vector3(0,0,180));
			
			GlobalFlags.canFire = false;
		}
	
	}
}
