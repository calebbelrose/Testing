using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : MonoBehaviour
{
	Vector3 translation;

	void Start ()
	{
		translation = new Vector3 (0.1f, 0.0f, 0.0f);
	}
	// Update is called once per frame
	void Update ()
	{
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
		gameObject.transform.Translate (translation);
	}
}
