using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
	public Transform car;

	private float distance = 10.0f;
	private float height = 3.0f;
	private float rotationDamping = 3.0f;
	private float heightDamping = 2.0f;
	private float zoomRatio = 50.0f;
	private float defaultFOV = 60.0f;
	private float wantedRotation;
	private Rigidbody rb;
	private Camera cam;

	void Start ()
	{
		rb = car.GetComponent<Rigidbody> ();
		cam = GetComponent<Camera> ();
	}

	void LateUpdate ()
	{
		float wantedHeight = car.position.y + height;
		float myRotation = transform.eulerAngles.y;
		float myHeight = transform.position.y;
		Quaternion currentRotation;

		myRotation = Mathf.LerpAngle(myRotation, wantedRotation, rotationDamping * Time.deltaTime);
		myHeight = Mathf.Lerp (myHeight, wantedHeight, heightDamping * Time.deltaTime);

		currentRotation = Quaternion.Euler (new Vector3 (0.0f, myRotation, 0.0f));
		transform.position = car.position;
		transform.position -= currentRotation* Vector3.forward * distance;
		transform.position = new Vector3 (transform.position.x, myHeight, transform.position.z);
		transform.LookAt (car);
	}

	void FixedUpdate()
	{
		Vector3 localVelocity = car.InverseTransformDirection (rb.velocity);

		if (localVelocity.z < -0.5f)
			wantedRotation = car.eulerAngles.y + 180.0f;
		else
			wantedRotation = car.eulerAngles.y;

		float acceleration = rb.velocity.magnitude;
		cam.fieldOfView = defaultFOV + acceleration * zoomRatio * Time.deltaTime;
	}
}
