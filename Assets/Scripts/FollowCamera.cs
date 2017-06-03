using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FollowCamera : NetworkBehaviour
{
	public GameObject character;
	public GameObject inventoryCanvas;
	public GameObject craftingCanvas;
	public GameObject mainCamera;
	public string playerName;

	private float xAxis;
	private float yAxis;
	private bool movementAllowed = true;
	private Animator animator;
	private float vertical;
	private const float speed = 2.5f;
	private bool thirdPerson = true;

	void Start()
	{
		inventoryCanvas.SetActive(false);
		craftingCanvas.SetActive(false);

		if (isLocalPlayer)
		{
			mainCamera.SetActive(true);
			playerName = "Player" + playerControllerId.ToString ();
			animator = character.GetComponent<Animator> ();
			gameObject.name = playerName;
		}
	}

	void Update()
	{
		if(isLocalPlayer)
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
					craftingCanvas.SetActive (false);
					movementAllowed = false;
				}
			}
			else if (Input.GetKeyDown (KeyCode.C))
			{
				if (craftingCanvas.activeSelf)
				{
					craftingCanvas.SetActive (false);
					movementAllowed = true;
				}
				else
				{
					craftingCanvas.SetActive (true);
					inventoryCanvas.SetActive (false);
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
			
				mainCamera.transform.rotation = rotation;

				if (thirdPerson)
				{
					target.y += 17.5f;
					mainCamera.transform.position = target + rotation * new Vector3 (0.0f, 0.0f, -25.0f);
				}
				else
				{
					target.y += 16.5f;
					mainCamera.transform.position = target + rotation * new Vector3 (0.0f, 0.0f, 1.0f);
				}

				vertical = Input.GetAxis ("Vertical");

				if(Input.GetKey(KeyCode.T))
					thirdPerson = !thirdPerson;

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
			}
	}
}