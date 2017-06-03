using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingSystem : MonoBehaviour
{
	public List<BuildObject> objects = new List<BuildObject>();
	public BuildObject currObject;
	public Transform preview;
	public Transform cam;
	public RaycastHit hit;
	public LayerMask layer;
	public float offset = 1.0f;
	public float gridSize = 1.0f;
	public bool isBuilding;
	public MCFace direction;
	public GameObject objMenuObject;

	private Vector3 currPosition;
	private Vector3 currRotation = new Vector3(0.0f, 0.0f, 0.0f);
	private float angle;
	private Vector3 south = new Vector3 (0.0f, -1.0f, -1.0f);
	private Vector3 north = new Vector3 (0.0f, -1.0f, 1.0f);
	private Vector3 up = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3 down = new Vector3 (1.0f, 1.0f, 1.0f);
	private Vector3 east = new Vector3 (1.0f, -1.0f, 0.0f);
	private Vector3 west = new Vector3 (-1.0f, -1.0f, 0.0f);
	private Vector3 northOffset = new Vector3 (0.0f, 0.0f, 2.0f);
	private Vector3 southOffset = new Vector3 (0.0f, 0.0f, -2.0f);
	private Vector3 eastOffset = new Vector3 (2.0f, 0.0f, 0.0f);
	private Vector3 westOffset = new Vector3 (-2.0f, 0.0f, 0.0f);
	private bool chooseObject;

	void Start()
	{
		ChangeCurrentBuilding (0);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		if (!chooseObject)
		{
			if (isBuilding)
				StartPreview ();

			if (Input.GetMouseButtonDown (0))
				Build ();
		}

		if (Input.GetMouseButtonDown (2) && !objMenuObject.activeSelf)
		{
			objMenuObject.SetActive (true);
			Cursor.lockState = CursorLockMode.None;
			chooseObject = true;
		}
		/*if (!Input.GetMouseButtonDown (2) && objMenuObject.activeSelf)
			DisableMenu ();*/
	}

	public void DisableMenu()
	{
		objMenuObject.SetActive (false);
		Cursor.lockState = CursorLockMode.Locked;
		chooseObject = false;
	}

	public void ChangeCurrentBuilding(int curr)
	{
		currObject = objects [curr];

		if (preview != null)
			Destroy (preview.gameObject);
		
		GameObject currPreview = Instantiate(currObject.preview, currPosition, Quaternion.Euler(currRotation)) as GameObject;
		preview = currPreview.transform;
	}

	public void StartPreview()
	{
		bool test = Physics.Raycast (cam.position, cam.forward, out hit, 10, layer);
		if (test)
		{
			if (hit.transform != this.transform)
				ShowPreview (hit);
		}
	}

	public void ShowPreview(RaycastHit hit2)
	{
		if (currObject.sort == ObjectSorts.floor)
		{
			direction = GetHitFace (hit2);
			if (direction == MCFace.Up || direction == MCFace.Down)
				currPosition = hit2.point;
			else
			{
				if (direction == MCFace.North)
					currPosition = hit2.point + northOffset;
				if (direction == MCFace.South)
					currPosition = hit2.point + southOffset;
				if (direction == MCFace.East)
					currPosition = hit2.point + eastOffset;
				if (direction == MCFace.West)
					currPosition = hit2.point + westOffset;
			}
		}
		else
			currPosition = hit2.point;

		if (hit2.transform.gameObject.CompareTag ("Pillar"))
		{
			if (hit2.transform.position.x < cam.position.x)
				currPosition.x += 2.0f;
			else if(hit2.transform.position.x > cam.position.x)
				currPosition.x -= 2.0f;

			if (hit2.transform.position.z < cam.position.z)
				currPosition.z += 2.0f;
			else if(hit2.transform.position.z > cam.position.z)
				currPosition.z -= 2.0f;
		}
		
		currPosition -= Vector3.one * offset;
		currPosition /= gridSize;
		currPosition = new Vector3(Mathf.Round(currPosition.x), Mathf.Round(currPosition.y), Mathf.Round(currPosition.z));
		currPosition *= gridSize;
		currPosition += Vector3.one * offset;
		preview.position = currPosition;

		if (Input.GetMouseButtonDown (1))
		{
			angle = (angle + 90.0f) % 360.0f;
			currRotation = new Vector3 (0.0f, angle, 0.0f);
		}

		preview.localEulerAngles = currRotation;
	}

	public void Build()
	{
		PreviewObject po = preview.GetComponent<PreviewObject> ();
		if (po.isBuildable)
		{
			Instantiate (currObject.prefab, currPosition, Quaternion.Euler(currRotation));
		}
	}

	public MCFace GetHitFace(RaycastHit hit)
	{
		Vector3 incomingVector = hit.normal - Vector3.up;

		if (incomingVector == south)
			return MCFace.South;
		if (incomingVector == north)
			return MCFace.North;
		if (incomingVector == up)
			return MCFace.Up;
		if (incomingVector == down)
			return MCFace.Down;
		if (incomingVector == west)
			return MCFace.West;
		if (incomingVector == east)
			return MCFace.East;

		return MCFace.None;
	}
}

[System.Serializable]
public class BuildObject
{
	public string name;
	public GameObject prefab;
	public GameObject preview;
	public ObjectSorts sort;
	public int gold;
}

public enum MCFace
{
	None,
	Up,
	Down,
	East,
	West,
	North,
	South
}