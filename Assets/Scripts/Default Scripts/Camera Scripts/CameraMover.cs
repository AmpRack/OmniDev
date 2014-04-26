// Attach to MainCamera, target to cameraFocalPoint on vehicle

using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {
	
	public Transform target;
	public float followDistance = 5.0f;
	public float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;
		
	void Start () {
	}

	void Update () {
		Vector3 targetPosition = new Vector3((target.position.x + followDistance), transform.position.y, transform.position.z);
		Vector3 targetRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

	}
}

