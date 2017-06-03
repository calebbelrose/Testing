using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
	public float destroyAfter;
	private float timer;

	void Update ()
	{
		timer += Time.deltaTime;
		if (destroyAfter <= timer)
			Destroy (gameObject);
	}
}
