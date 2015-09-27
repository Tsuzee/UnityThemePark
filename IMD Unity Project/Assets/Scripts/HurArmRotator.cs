using UnityEngine;
using System.Collections;

public class HurArmRotator : MonoBehaviour {

	private Vector3 rotationAxis = Vector3.right;
	private float angleOfRotation = 0.0f;
	private float angle = -10f;
	public float rideTime = 0f;
	private bool top = false;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (rideTime > 12f) {

			if (angleOfRotation < 70f && !top) {
				transform.Rotate (rotationAxis, angle * Time.deltaTime);
				angleOfRotation += (rotationAxis.x * Time.deltaTime) * -angle;
			} else if (!top) {
				angleOfRotation += (rotationAxis.x * Time.deltaTime) * -angle;
			}

			if (angleOfRotation >= 140f && !top) {
				angleOfRotation = 70f;
				top = true;
			}

			if (angleOfRotation > 0f && top) {
				transform.Rotate (rotationAxis, -angle * Time.deltaTime);
				angleOfRotation -= (rotationAxis.x * Time.deltaTime) * -angle;
			}

			if (angleOfRotation <= 0f && top) {

			}

			if (rideTime > 46f) {
				top = false;
				rideTime = 0f;
			}

		}
		rideTime += 1f * Time.deltaTime;
	}
}
