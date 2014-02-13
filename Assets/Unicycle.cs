using UnityEngine;
using System.Collections;

public class Unicycle : MonoBehaviour {

	public float offsetX = 0.0f;
	public float offsetY = 0.0f;
	public float offsetZ = 0.0f;
	public float motor_max = 10.0f;
	public float brake_max = 100.0f;
	public WheelCollider uniWheelCol;

	private GameObject uniWheel;
	private GameObject pedalLeft;
	private GameObject pedalRight;
	private float motor = 0.0f;
	private float brake = 0.0f;

	void Start () {
		rigidbody.centerOfMass = new Vector3(offsetX, offsetY, offsetZ);
		uniWheel = GameObject.Find("WheelAssembly");
		pedalLeft = GameObject.Find("PedalLeft");
		pedalRight = GameObject.Find("PedalRight");
	}

	
	void FixedUpdate () {
		motor = -1 * Mathf.Clamp (Input.GetAxis("Horizontal"), 0, 1);
		brake = -1 * Mathf.Clamp (Input.GetAxis("Horizontal"), -1, 0);
		uniWheelCol.motorTorque = motor_max * motor;
		uniWheelCol.brakeTorque = brake_max * brake;

		if (Input.GetKey ("right")) {
			uniWheel.transform.Rotate (-400 * Time.deltaTime, 0, 0);
			pedalLeft.transform.Rotate (400 * Time.deltaTime, 0, 0);
			pedalRight.transform.Rotate (-400 * Time.deltaTime, 0, 0);
		}

		if (Input.GetKey("left")) {
			uniWheel.transform.Rotate (400 * Time.deltaTime, 0, 0);
			pedalLeft.transform.Rotate (-400 * Time.deltaTime, 0, 0);
			pedalRight.transform.Rotate (400 * Time.deltaTime, 0, 0);
		}
	}
}
	