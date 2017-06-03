using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	public float blastRadius;
	public float damage;

	private AudioSource a;
	private Renderer r;

	void Start ()
	{
		a = GetComponent<AudioSource> ();
		r = GetComponent<Renderer> ();
		Invoke("Explode", 5.0f);
	}
	
	private void Explode()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

		foreach (Collider coll in colliders)
		{
			if (coll.gameObject.layer == 10 || coll.gameObject.layer == 9)
				coll.gameObject.GetComponent<Health> ().Damage (Vector3.Distance(transform.position, coll.gameObject.transform.position) / blastRadius * damage);
		}

		a.Play ();
		r.enabled = false;
		Invoke ("Delete", a.clip.length);
	}

	private void Delete()
	{
		Destroy (gameObject);
	}
}
