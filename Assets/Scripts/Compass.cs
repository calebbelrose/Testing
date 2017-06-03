using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
	public Quaternion missionDirection;
	public RectTransform northLayer;
	public RectTransform missionLayer;
	public Transform missionPlace;

	private  Vector3 northDirection;

	void Update ()
	{
		ChangeNorthDirection ();
		ChangeMissionDirection ();
	}

	private void ChangeNorthDirection()
	{
		northDirection.z = transform.eulerAngles.y;
		northLayer.localEulerAngles = northDirection;
	}

	private void ChangeMissionDirection()
	{
		if (Vector3.Distance (transform.position, missionPlace.position) > 2.5f)
		{
			missionLayer.gameObject.SetActive (true);
			Vector3 dir = transform.position - missionPlace.position;
			missionDirection = Quaternion.LookRotation (dir);
			missionDirection.z = -missionDirection.y;
			missionDirection.x = 0.0f;
			missionDirection.y = 0.0f;

			missionLayer.localRotation = missionDirection * Quaternion.Euler (northDirection);
		}
		else
			missionLayer.gameObject.SetActive (false);
	}
}
