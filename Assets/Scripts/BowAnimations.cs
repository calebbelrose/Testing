using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAnimations : MonoBehaviour {
	Animation animations;
	bool ready = false, pulledBack = false;
	// Use this for initialization
	void Start ()
	{
		animations = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (1))
		{
			ready = !ready;
			if (ready)
				animations.Play ("Idle");
			else
				animations.Play ("MoveToSide");
		}
		else if (ready)
		{
			if (Input.GetMouseButtonDown (0))
			{
				animations.Play ("PullBack");
				pulledBack = true;
			}
			else if (pulledBack && Input.GetMouseButtonUp (0))
			{
				animations.Play ("Idle");
				pulledBack = false;
			}
		}
	}
}
