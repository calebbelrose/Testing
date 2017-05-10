using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	public GameObject character;
	public GameObject inventoryCanvas;

	private float xAxis;
	private float yAxis;
	private bool movementAllowed = true;
	private Animator animator;
	private float vertical;
	private float horizontal;

	private const float distance = 25.0f;
	private const float speed = 2.5f;

	void Start()
	{
		inventoryCanvas.SetActive (false);
		animator = character.GetComponent<Animator> ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.I))
		{
			if (inventoryCanvas.activeSelf)
			{
				inventoryCanvas.SetActive (false);
				movementAllowed = true;
			}
			else
			{
				inventoryCanvas.SetActive (true);
				movementAllowed = false;
			}
		}
		else if (movementAllowed)
		{
			float x = Input.GetAxis ("Mouse X") * speed;

			xAxis += x * speed;
			yAxis -= Input.GetAxis ("Mouse Y") * speed;

			Vector3 target = character.transform.position;
			Quaternion rotation = Quaternion.Euler (yAxis, xAxis, 0.0f);

			character.transform.Rotate (0.0f, x * speed, 0.0f);
		
			target.y += 17.5f;
			transform.rotation = rotation;
			transform.position = target + rotation * new Vector3 (0.0f, 0.0f, -distance);

			vertical = Input.GetAxis ("Vertical");
			horizontal = Input.GetAxis ("Horizontal");

			if (Input.GetKey (KeyCode.LeftControl))
			{
				animator.SetBool ("crawling", true);
				animator.SetBool ("sneaking", false);
				animator.SetBool ("sprinting", false);
			}
			else if (Input.GetKey (KeyCode.Z))
			{
				animator.SetBool ("sneaking", true);
				animator.SetBool ("crawling", false);
				animator.SetBool ("sprinting", false);
			}
			else if (Input.GetKey (KeyCode.LeftShift))
			{
				animator.SetBool ("sprinting", true);
				animator.SetBool ("crawling", false);
				animator.SetBool ("sneaking", false);
			}
			else
			{
				animator.SetBool ("crawling", false);
				animator.SetBool ("sneaking", false);
				animator.SetBool ("sprinting", false);
			}

			animator.SetFloat ("walk", vertical);
			animator.SetFloat ("turn", horizontal);
		}
	}
}