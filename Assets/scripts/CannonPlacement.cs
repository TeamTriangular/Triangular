using UnityEngine;
using System.Collections;

/**
 * Script for moving the cannon. Will be able to click within the vicinity of the cannon and have it move to that location 
 * based on the offset.
 */
public class CannonPlacement : MonoBehaviour {
	
	public float sizeOffset = 2;
	public float moveOffset = 2;
	
	void FixedUpdate () 
	{
		if (!GlobalFlags.getPaused()){
	 		GameObject cannon = GameObject.FindGameObjectWithTag("Player");
			if (Input.GetMouseButton(0))
			{
				Plane playerPlane = new Plane(Vector3.forward, transform.position);
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				float hitdist = 0;
				
				if(playerPlane.Raycast(ray, out hitdist))
				{
					Vector3 target = ray.GetPoint(hitdist);
					
					float mouseDist = Vector3.Distance(transform.position, target);
					float cannonDist = Vector3.Distance(transform.position, cannon.transform.position);
					
					if(mouseDist < cannonDist - sizeOffset)
					{
						GlobalFlags.canFire = true;
						float y = (target.y - cannon.transform.position.y);
						float x = (target.x - cannon.transform.position.x);
						float atan = Mathf.Atan2(y, x);
				
						cannon.transform.rotation = Quaternion.Euler(new Vector3(0,0,atan * Mathf.Rad2Deg - 90));
					}
					else if(Vector3.Distance(target, cannon.transform.position) < moveOffset)
					{
						GlobalFlags.canFire = false;
						float y = (target.y - transform.position.y);
						float x = (target.x - transform.position.x);
						float atan = Mathf.Atan2(y, x);
				
						transform.rotation = Quaternion.Euler(new Vector3(0,0,atan * Mathf.Rad2Deg - 90));
					}
				}
			}
		}
	}
}
