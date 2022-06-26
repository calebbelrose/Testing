using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupObject : MonoBehaviour
{

	bool carrying = false;
	GameObject carriedObject;
	float distance = 1.0f;
	float smooth = 4.0f;
	
	// Update is called once per frame
	void Update ()
	{
		//if(isLocalPlayer)
		if (carrying)
			{
				Carry (carriedObject);
				CheckDrop ();
			}
			else
				Pickup();
	}

	void Carry(GameObject o)
	{
		o.transform.position = Vector3.Lerp(o.transform.position, Camera.main.transform.position + Camera.main.transform.forward * distance, Time.deltaTime * smooth);
	}

	void Pickup()
	{
		if (Input.GetKeyDown (KeyCode.E))
		{
			int x = Screen.width / 2;
			int y = Screen.height / 2;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3(x, y));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1.0f))
			{
				Pickupable p = hit.collider.GetComponent<Pickupable> ();
				distance = hit.distance;
				if (p != null)
				{
					carrying = true;
					carriedObject = p.gameObject;
					carriedObject.GetComponent<Rigidbody> ().isKinematic = true;
				}
			}
		}
	}

	void CheckDrop()
	{
		if (Input.GetKeyDown (KeyCode.E))
			Drop ();
	}

	void Drop()
	{
		carrying = false;
		carriedObject.GetComponent<Rigidbody> ().isKinematic = false;
		carriedObject = null;
	}
}
