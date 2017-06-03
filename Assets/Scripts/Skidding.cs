using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skidding : MonoBehaviour
{
	public GameObject skidSound;
	public Material skidMaterial;
	public ParticleSystem skidSmoke;
	public bool rearWheel;

	private float currentFriction;
	private float skidAt = 0.15f;
	private float soundEmission = 15.0f;
	private float soundWait = 0.0f;
	private float markWidth = 0.2f;
	private bool skidding;
	private Vector3[] lastPos = new Vector3[2];

	void Update ()
	{
		WheelHit hit;
		WheelCollider wc = GetComponent<WheelCollider> ();
		float rpm = wc.rpm;
		bool vertical = Input.GetAxis ("Vertical") > 0.0f;
		wc.GetGroundHit (out hit);
		currentFriction = Mathf.Abs(hit.sidewaysSlip);

		if ((currentFriction >= skidAt && soundWait <= 0.0f) || (rearWheel && rpm < 200 && vertical && soundWait <= 0.0f && hit.collider))
		{
			Instantiate (skidSound, hit.point, Quaternion.identity);
			soundWait = 1.0f;
		}

		soundWait -= Time.deltaTime * soundEmission;

		if (currentFriction >= skidAt || (rearWheel && rpm < 200 && vertical && hit.collider))
		{
			skidSmoke.Play ();
			SkidMesh ();
		}
		else
		{
			skidSmoke.Stop ();
			skidding = false;
		}
	}

	void SkidMesh()
	{
		WheelHit hit;
		MeshFilter meshFilter;
		MeshRenderer meshRenderer;

		GetComponent<WheelCollider> ().GetGroundHit (out hit);
		GameObject mark = new GameObject ("Mark");
		meshFilter = mark.AddComponent<MeshFilter>();
		meshRenderer = mark.AddComponent<MeshRenderer>();
		Mesh markMesh = new Mesh ();
		Vector3[] vertices = new Vector3[4];
		int[] triangles = new int[6];

		if (skidding)
		{
			vertices [0] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (markWidth, 0.1f, 0.0f);
			vertices [1] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (-markWidth, 0.1f, 0.0f);
			vertices [2] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (-markWidth, 0.1f, 0.0f);
			vertices [3] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (markWidth, 0.1f, 0.0f);
			lastPos [0] = vertices [2];
			lastPos [1] = vertices [3];
			skidding = true;
		}
		else
		{
			vertices [0] = lastPos [1];
			vertices [1] = lastPos [0];
			vertices [2] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (-markWidth, 0.1f, 0.0f);
			vertices [3] = hit.point + Quaternion.Euler (transform.eulerAngles) * new Vector3 (markWidth, 0.1f, 0.0f);
			lastPos [0] = vertices [2];
			lastPos [1] = vertices [3];
		}
		triangles = new int[] {0, 1, 2, 2, 3, 0};
		markMesh.vertices = vertices;
		markMesh.triangles = triangles;
		Vector2[] uvm = new Vector2[4];

		uvm [0] = new Vector2 (1.0f, 0.0f);
		uvm [1] = new Vector2 (0.0f, 0.0f);
		uvm [2] = new Vector2 (0.0f, 1.0f);
		uvm [3] = new Vector2 (1.0f, 1.0f);

		markMesh.uv = uvm;
		meshFilter.mesh = markMesh;
		meshRenderer.material = skidMaterial;

		mark.AddComponent<DestroyTimer> ();
	}
}
