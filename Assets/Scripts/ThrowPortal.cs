using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPortal : MonoBehaviour {
	public GameObject [] portals = new GameObject[2];
	private int currPortal = 0;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
			ThrowAPortal ();
	}

	void ThrowAPortal()
	{
		Transform camera = GetComponent<Camera> ().transform;
		RaycastHit hit;

		if (Physics.Raycast (camera.position, camera.forward, out hit, 500.0f))
		{
			Quaternion rotation = Quaternion.LookRotation (hit.normal);
			portals [currPortal].transform.position = hit.point;
			portals [currPortal].transform.rotation = rotation;
			currPortal = (currPortal + 1) % 2;
		}
	}
}
