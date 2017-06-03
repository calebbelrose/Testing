using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GrapplingHook : MonoBehaviour
{
	public Camera cam;
	public LayerMask cullingMask;
	public Transform hand;
	public FirstPersonController fpc;
	public LineRenderer lr;
	public GameObject hookPrefab;

	private bool isGrappling;
	private Vector3 location;
	private RaycastHit hit;
	private float speed = 0.0f;
	private float maxDistance = 25.0f;
	private GameObject hook;

	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.G))
			FindLocation();

		if(isGrappling)
			Grappling();

		if(Input.GetKeyDown(KeyCode.Space) && isGrappling)
			StopGrappling();
	}

	public void FindLocation()
	{
		if (Physics.Raycast (cam.transform.position, cam.transform.forward, out hit, maxDistance, cullingMask))
		{
			isGrappling = true;
			location = hit.point;
			fpc.canMove = false;
			lr.enabled = true;
			lr.SetPosition (0, location);
			hook = Instantiate (hookPrefab, location, Quaternion.LookRotation((location - transform.position).normalized));
		}
	}

	public void Grappling()
	{
		transform.position = Vector3.Lerp (transform.position, location, speed * Time.deltaTime / Vector3.Distance(transform.position, location));
		lr.SetPosition (1, hand.position);

		if (Vector3.Distance (transform.position, location) < 0.5f)
			StopGrappling ();
	}

	public void StopGrappling()
	{
		isGrappling = false;
		fpc.canMove= false;
		lr.enabled = false;
		Destroy (hook);
	}
}
