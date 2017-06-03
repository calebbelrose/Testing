using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelBL;
	public WheelCollider wheelBR;
	public Transform wheelFLTrans;
	public Transform wheelFRTrans;
	public Transform wheelBLTrans;
	public Transform wheelBRTrans;
	public GameObject taillight;
	public Material tailIdleMaterial;
	public Material tailBrakeMaterial;
	public Material tailReverseMaterial;
	public int[] gearRatio;
	public Texture2D speedometerDial;
	public Texture2D speedometerPointer;
	public GameObject spark;
	public GameObject collisionSound;

	private float maxTorque = 50.0f;
	private float lowestSteerAtSpeed = 50.0f;
	private float lowSpeedSteerAngle = 10.0f;
	private float highSpeedSteerAngle = 1.0f;
	private float decelerationSpeed = 30.0f;
	private Rigidbody rb;
	public float currentSpeed;
	private float maxForwardSpeed = 300.0f;
	private float maxReverseSpeed = -100.0f;
	private Renderer taillightRenderer;
	private Material[] taillightMaterials;
	private bool braking = false;
	private float maxBrakeTorque = 100.0f;
	public float myForwardFriction;
	public float mySidewaysFriction;
	private float slipForwardFriction = 0.50f;
	private float slipSidewaysFriction = 0.85f;
	private AudioSource engineAudio;
	private Rect dialRect = new Rect (Screen.width - 300.0f, Screen.height - 150.0f, 300.0f, 150.0f);
	private Rect pointerRect = new Rect (Screen.width - 300.0f, Screen.height - 150.0f, 300.0f, 300.0f);

	public bool test = false;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		rb.centerOfMass = new Vector3 (0.0f, -2.0f, 0.5f);
		taillightRenderer = taillight.GetComponent<Renderer> ();
		taillightMaterials = taillightRenderer.materials;
		engineAudio = gameObject.GetComponent<AudioSource> ();
		SetValues ();
	}

	void SetValues()
	{
		myForwardFriction = wheelBL.forwardFriction.stiffness;
		mySidewaysFriction = wheelBL.sidewaysFriction.stiffness;
	}
		
	void FixedUpdate ()
	{
		Control ();
		HandBrake ();
	}

	void Update()
	{
		wheelFLTrans.Rotate (wheelFL.rpm * 6 * Time.deltaTime, 0.0f, 0.0f);
		wheelFRTrans.Rotate (wheelFR.rpm * 6 * Time.deltaTime, 0.0f, 0.0f);
		wheelBLTrans.Rotate (wheelBL.rpm * 6 * Time.deltaTime, 0.0f, 0.0f);
		wheelBRTrans.Rotate (wheelBR.rpm * 6 * Time.deltaTime, 0.0f, 0.0f);
		wheelFLTrans.localEulerAngles = new Vector3(wheelFLTrans.localEulerAngles.x, wheelFL.steerAngle - wheelFLTrans.localEulerAngles.z, wheelFLTrans.localEulerAngles.z);
		wheelFRTrans.localEulerAngles = new Vector3(wheelFRTrans.localEulerAngles.x, wheelFR.steerAngle - wheelFRTrans.localEulerAngles.z, wheelFRTrans.localEulerAngles.z);

		Taillight ();
		WheelPosition ();
		EngineSound ();
	}

	void Control()
	{
		currentSpeed = Mathf.Round(Mathf.PI * wheelBL.radius * wheelBL.rpm * 0.012f);
		float vertical = Input.GetAxis ("Vertical");

		if (currentSpeed < maxForwardSpeed && currentSpeed > maxReverseSpeed && !braking) {
			wheelBL.motorTorque = maxTorque * vertical;
			wheelBR.motorTorque = maxTorque * vertical;
		}
		else
		{
			wheelBL.motorTorque = 0.0f;
			wheelBR.motorTorque = 0.0f;
		}

		if (Input.GetButton ("Vertical") == false)
		{
			wheelBL.brakeTorque = decelerationSpeed;
			wheelBR.brakeTorque = decelerationSpeed;
		}
		else
		{
			wheelBL.brakeTorque = 0.0f;
			wheelBR.brakeTorque = 0.0f;
		}

		float speedFactor = rb.velocity.magnitude / lowestSteerAtSpeed;
		float currentSteerAngle = Mathf.Lerp (lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * Input.GetAxis ("Horizontal");

		wheelFL.steerAngle = currentSteerAngle;
		wheelFR.steerAngle = currentSteerAngle;
	}

	void Taillight()
	{
		if (!braking)
		{
			float vertical = Input.GetAxis ("Vertical");

			if ((currentSpeed > 0.0f && vertical < 0.0f) || (currentSpeed < 0.0f && vertical > 0.0f))
				taillightMaterials [1] = tailBrakeMaterial;
			else if (currentSpeed < 0.0f && vertical < 0.0f)
				taillightMaterials [1] = tailReverseMaterial;
			else
				taillightMaterials [1] = tailIdleMaterial;
		
			taillightRenderer.materials = taillightMaterials;
		}
	}

	void WheelPosition()
	{
		RaycastHit hit;
		float radius = 0.52f;

		if (Physics.Raycast (wheelFL.transform.position, -wheelFL.transform.up, out hit, radius + wheelFL.suspensionDistance))
			wheelFLTrans.position = hit.point + wheelFL.transform.up * radius;
		else
			wheelFLTrans.position = wheelFL.transform.position - wheelFL.transform.up * wheelFL.suspensionDistance;
		
		if (Physics.Raycast (wheelFR.transform.position, -wheelFR.transform.up, out hit, radius + wheelFR.suspensionDistance))
			wheelFRTrans.position = hit.point + wheelFR.transform.up * radius;
		else
			wheelFRTrans.position = wheelFR.transform.position - wheelFR.transform.up * wheelFR.suspensionDistance;

		if (Physics.Raycast (wheelBL.transform.position, -wheelBL.transform.up, out hit, radius + wheelBL.suspensionDistance))
			wheelBLTrans.position = hit.point + wheelBL.transform.up * radius;
		else
			wheelBLTrans.position = wheelBL.transform.position - wheelBL.transform.up * wheelBL.suspensionDistance;

		if (Physics.Raycast (wheelBR.transform.position, -wheelBR.transform.up, out hit, radius + wheelBR.suspensionDistance))
			wheelBRTrans.position = hit.point + wheelBR.transform.up * radius;
		else
			wheelBRTrans.position = wheelBR.transform.position - wheelBR.transform.up * wheelBR.suspensionDistance;
	}

	void HandBrake()
	{
		if (Input.GetButton ("Jump"))
			braking = true;
		else
			braking = false;

		if (braking)
		{
			wheelFL.brakeTorque = maxBrakeTorque;
			wheelFR.brakeTorque = maxBrakeTorque;
			wheelBL.motorTorque = 0.0f;
			wheelBR.motorTorque = 0.0f;

			if (rb.velocity.magnitude > 1.0f)
				SetSlip (slipForwardFriction, slipSidewaysFriction);
			else
				SetSlip (0.1f, 0.5f);

			if (currentSpeed < 1.0f && currentSpeed > -1.0f)
				taillightMaterials [1] = tailIdleMaterial;
			else
				taillightMaterials [1] = tailBrakeMaterial;

			taillightRenderer.materials = taillightMaterials;
		}
		else
		{
			wheelFL.brakeTorque = 0.0f;
			wheelFR.brakeTorque = 0.0f;
			SetSlip (myForwardFriction, mySidewaysFriction);
		}
	}

	void SetSlip(float currentForwardFriction, float currentSidewaysFriction)
	{
		wheelFL.forwardFriction = SetFriction (wheelFL.forwardFriction, currentForwardFriction);
		wheelFR.forwardFriction = SetFriction (wheelFR.forwardFriction, currentForwardFriction);
		wheelBL.forwardFriction = SetFriction (wheelBL.forwardFriction, currentForwardFriction);
		wheelBR.forwardFriction = SetFriction (wheelBR.forwardFriction, currentForwardFriction);

		wheelFL.sidewaysFriction = SetFriction (wheelFL.sidewaysFriction, currentSidewaysFriction);
		wheelFR.sidewaysFriction = SetFriction (wheelFR.sidewaysFriction, currentSidewaysFriction);
		wheelBL.sidewaysFriction = SetFriction (wheelBL.sidewaysFriction, currentSidewaysFriction);
		wheelBR.sidewaysFriction = SetFriction (wheelBR.sidewaysFriction, currentSidewaysFriction);
	}

	WheelFrictionCurve SetFriction(WheelFrictionCurve wfc, float value)
	{
		wfc.stiffness = value;
		return wfc;
	}

	void EngineSound()
	{
		int gear;
		float gearMinValue, gearMaxValue;

		for (gear = 0; gear < gearRatio.Length; gear++)
			if (currentSpeed <= gearRatio [gear])
				break;

		if (gear == 0)
		{
			gearMinValue = 0.0f;
			gearMaxValue = gearRatio [0];
		}
		else if (gear == gearRatio.Length)
		{
			gearMinValue = gearRatio [gearRatio.Length - 2];
			gearMaxValue = gearRatio [gearRatio.Length - 1];
		}
		else
		{
			gearMinValue = gearRatio [gear - 1];
			gearMaxValue = gearRatio [gear];
		}

		float enginePitch = ((currentSpeed - gearMinValue) / (gearMaxValue - gearMinValue)) + 0.5f;
		engineAudio.pitch = enginePitch;
	}

	void OnGUI()
	{
		pointerRect.x = (dialRect.x = Screen.width - 300.0f);
		pointerRect.y = (dialRect.y = Screen.height - 150.00f);
		GUI.DrawTexture (dialRect, speedometerDial);
		float speedFactor = Mathf.Abs(currentSpeed) / maxForwardSpeed;
		float rotationAngle = Mathf.Lerp (0.0f, 180.0f, speedFactor);
		GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 150.0f, Screen.height));
		GUI.DrawTexture (pointerRect, speedometerPointer);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.transform != transform && other.contacts.Length != 0)
			for (int i = 0; i < other.contacts.Length; i++)
			{
				Instantiate (spark, other.contacts [i].point, Quaternion.identity);
				Instantiate (collisionSound, other.contacts [i].point, Quaternion.identity);
			}
	}
}