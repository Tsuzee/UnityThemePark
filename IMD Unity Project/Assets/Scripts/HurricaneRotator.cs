using UnityEngine;
using System.Collections;

public class HurricaneRotator : MonoBehaviour {

	private Vector3 rotationAxis = Vector3.down;
	private float rideTime = 0f;
	private Vector3 moveStep = new Vector3(0,0,0);
	private float rotationSpeed = 30.0f;
	public GameObject ride;
	public GameObject step1;
	public GameObject step2;
	public GameObject step3;
	public GameObject step4;
	public GameObject step5;

	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if( rideTime > 12f && rideTime < 28 && rotationSpeed < 90f)
		{
			rotationSpeed += 8f * Time.deltaTime;
		}
		else if( rideTime > 28f && rotationSpeed > 30f)
		{
			rotationSpeed -= 8f * Time.deltaTime;
		}

		if (rideTime > 6f && rideTime < 46f) {
			transform.Rotate (rotationAxis, rotationSpeed * Time.deltaTime);

		}

		//move steps to cars out of the way, and back in again
		if (rideTime > 1f && rideTime < 7f) {
			//step 1
			moveStep = step1.transform.position;
			moveStep[2] += -.5f * Time.deltaTime;
			step1.transform.position = moveStep;

			//step 2
			moveStep = step2.transform.position;
			moveStep[0] += -.6f * Time.deltaTime;
			step2.transform.position = moveStep;

			//step 3
			moveStep = step3.transform.position;
			moveStep[0] += -.4f * Time.deltaTime;
			step3.transform.position = moveStep;

			//step 4
			moveStep = step4.transform.position;
			moveStep[0] += .45f * Time.deltaTime;
			step4.transform.position = moveStep;

			//step 5
			moveStep = step5.transform.position;
			moveStep[0] += .5f * Time.deltaTime;
			step5.transform.position = moveStep;
		}

		if (rideTime > 38f && rideTime < 45f) {
			moveStep = step1.transform.position;
			moveStep[2] += .5f * Time.deltaTime;
			step1.transform.position = moveStep;

			//step 2
			moveStep = step2.transform.position;
			moveStep[0] += .6f * Time.deltaTime;
			step2.transform.position = moveStep;
			
			//step 3
			moveStep = step3.transform.position;
			moveStep[0] += .4f * Time.deltaTime;
			step3.transform.position = moveStep;
			
			//step 4
			moveStep = step4.transform.position;
			moveStep[0] += -.45f * Time.deltaTime;
			step4.transform.position = moveStep;
			
			//step 5
			moveStep = step5.transform.position;
			moveStep[0] += -.5f * Time.deltaTime;
			step5.transform.position = moveStep;
		}

		if (rideTime > 46f) {
			rideTime = 0f;
			//ride.transform.Rotate(new Vector3(0, 0.05759f, 0));
			ride.transform.rotation = Quaternion.identity;
		}

		rideTime += 1f * Time.deltaTime;
	}
}
