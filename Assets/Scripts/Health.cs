using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float health;

	public void Damage(float damage)
	{
		health -= damage;
		if (health <= 0.0f)
			Destroy (gameObject);
	}
}
