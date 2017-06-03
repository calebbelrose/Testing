using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
	public Transform path;
	public float maxSteerAngle = 10.0f;
	public float maxMotorTorque = 50.0f;
	public float maxBrakeTorque = 100.0f;
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelBL;
	public WheelCollider wheelBR;
	public float currSpeed;
	public float maxSpeed = 300.0f;
	public Material tailIdleMaterial;
	public Material tailBrakeMaterial;
	public Material tailReverseMaterial;
	public Material[] materials;
	public GameObject taillight;

	[Header("Sensors")]
	public float sensorLength = 10.0f;

	private List<Transform> nodes;
	private int currNode = 0;
	private bool isBraking;
	private Renderer carRenderer;
	private float frontSensorPosition = 3.79f;
	private float sideSensorPosition = 1.475f;
	private float frontSensorAngle = 30.0f;
	private bool avoiding = false;
	private float targetSteerAngle = 0.0f;
	private float turnSpeed = 5.0f;

	void Start ()
	{
		GetComponent<Rigidbody> ().centerOfMass = new Vector3 (0.0f, -2.0f, 0.5f);
		Transform[] pathTransforms = path.GetComponentsInChildren<Transform> ();
		nodes = new List<Transform> ();

		for (int i = 0; i < pathTransforms.Length; i++)
			if (pathTransforms [i] != path.transform)
				nodes.Add (pathTransforms [i]);

		carRenderer = taillight.GetComponent<Renderer> ();
		materials = carRenderer.materials;
	}

	void FixedUpdate ()
	{
		Sensors ();
		ApplySteer ();
		Drive ();
		CheckDistance ();
		Braking ();
		LerpToSteerAngle ();
	}

	void Sensors()
	{
		RaycastHit hit;
		Vector3 sensorStartingPosition = transform.position;
		float avoidMultiplier = 0.0f;
		avoiding = false;
		sensorStartingPosition += transform.forward * frontSensorPosition;
	

		// Front right sensor
		sensorStartingPosition += transform.right * sideSensorPosition;
		if (Physics.Raycast (sensorStartingPosition, transform.forward, out hit, sensorLength))
		{
			if(!hit.collider.CompareTag("Terrain"))
			{
				avoiding = true;
				Debug.DrawLine (sensorStartingPosition, hit.point);
				avoidMultiplier -= 1.0f;
			}
		}

		// Front right angle sensor
		else if (Physics.Raycast (sensorStartingPosition, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
		{
			if(!hit.collider.CompareTag("Terrain"))
			{
				avoiding = true;
				Debug.DrawLine (sensorStartingPosition, hit.point);
				avoidMultiplier -= 0.5f;
			}
		}

		// Front left sensor
		sensorStartingPosition -= transform.right * sideSensorPosition * 2;
		if (Physics.Raycast (sensorStartingPosition, transform.forward, out hit, sensorLength))
		{
			if(!hit.collider.CompareTag("Terrain"))
			{
				Debug.DrawLine (sensorStartingPosition, hit.point);
				avoiding = true;
				avoidMultiplier += 0.5f;
			}
		}

		// Front left angle sensor
		else if (Physics.Raycast (sensorStartingPosition, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
		{
			if(!hit.collider.CompareTag("Terrain"))
			{
				avoiding = true;
				Debug.DrawLine (sensorStartingPosition, hit.point);
				avoidMultiplier += 1.0f;
			}
		}

		// Front center sensor
		if(avoidMultiplier == 0.0f)
			if (Physics.Raycast (sensorStartingPosition, transform.forward, out hit, sensorLength))
			{
				if(!hit.collider.CompareTag("Terrain"))
				{
					avoiding = true;
					Debug.DrawLine (sensorStartingPosition, hit.point);
					if (hit.normal.x < 0.0f)
						avoidMultiplier = -1.0f;
					else
						avoidMultiplier = 1.0f;
				}
			}

		if (avoiding)
			targetSteerAngle = maxSteerAngle * avoidMultiplier;
	}

	void ApplySteer()
	{
		if (avoiding)
			return;
		
		Vector3 relativeVector = transform.InverseTransformPoint (nodes[currNode].position);
		relativeVector /= relativeVector.magnitude;
		float newSteer = relativeVector.x / relativeVector.magnitude * maxSteerAngle;
		targetSteerAngle = newSteer;
	}

	void Drive()
	{
		currSpeed = 2 * Mathf.PI * wheelBL.radius * 0.012f;
		if (currSpeed < maxSpeed && !isBraking)
		{
			wheelBL.motorTorque = maxMotorTorque;
			wheelBR.motorTorque = maxMotorTorque;
		}
		else
		{
			wheelBL.motorTorque = 0.0f;
			wheelBR.motorTorque = 0.0f;
		}
	}

	void CheckDistance()
	{
		if (Vector3.Distance (transform.position, nodes [currNode].position) < 5.0f)
			if (currNode == nodes.Count - 1)
				currNode = 0;
			else
				currNode++;
	}

	void Braking()
	{
		if (isBraking)
		{
			materials [1] = tailBrakeMaterial;
			carRenderer.materials = materials;
			wheelBL.brakeTorque = maxBrakeTorque;
			wheelBR.brakeTorque = maxBrakeTorque;
		}
		else
		{
			materials [1] = tailIdleMaterial;
			carRenderer.materials = materials;
			wheelBL.brakeTorque = 0.0f;
			wheelBR.brakeTorque = 0.0f;
		}
	}

	void LerpToSteerAngle()
	{
		wheelFL.steerAngle = Mathf.Lerp (wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
		wheelFR.steerAngle = Mathf.Lerp (wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
	}
}
