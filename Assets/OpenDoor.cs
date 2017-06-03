using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	public GameObject door;

	private float smooth = 2.0f;
	private float DoorOpenAngle = 90.0f;
	private bool open;
	private bool enter;
	private Vector3 defaultRot;
	private Vector3 openRot;

	void Start()
	{
		defaultRot = transform.eulerAngles;
		openRot = new Vector3 (defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
	}

	//Main function
	void Update ()
	{
		if(open)
			door.transform.eulerAngles = Vector3.Slerp(door.transform.eulerAngles, openRot, Time.deltaTime * smooth);
		else
			door.transform.eulerAngles = Vector3.Slerp(door.transform.eulerAngles, defaultRot, Time.deltaTime * smooth);

		if(Input.GetKeyDown("f") && enter)
			open = !open;
	}

	void OnGUI()
	{
		if(enter)
			GUI.Label(new Rect(Screen.width/2 - 75, Screen.height - 100, 150, 30), "Press 'F' to open the door");
	}

	//Activate the Main function when player is near the door
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Player")
			enter = true;
	}

//Deactivate the Main function when player is go away from door
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "Player")
			enter = false;
	}
}
