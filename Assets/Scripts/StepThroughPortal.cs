using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepThroughPortal : MonoBehaviour {
	public GameObject otherPortal;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.transform.position = new Vector3 (otherPortal.transform.position.x + otherPortal.transform.forward.x * 5, otherPortal.transform.position.y + otherPortal.transform.forward.y * 5 - 10.0f, otherPortal.transform.position.z + otherPortal.transform.forward.z * 5);
			Quaternion rotation = Quaternion.FromToRotation(otherPortal.transform.forward, transform.forward);
			other.transform.rotation *= rotation;
		}
	}
}
