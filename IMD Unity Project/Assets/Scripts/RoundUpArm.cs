using UnityEngine;
using System.Collections;

public class RoundUpArm : MonoBehaviour 
{
    private Vector3 rotationAxis = Vector3.right;
    private float angleOfRotation = 0.0f;
    private float angle = 2f;
    public float rideTime = 0f;
    private bool top = false;
    public GameObject RotatingRide;

    
    // Use this for initialization
    void Start () 
    {
    }
    
    // Update is called once per frame
    void Update () 
    {
        if (rideTime > 4f)
        {
            //start the round up spinning
            RotatingRide.GetComponent<RotateClockwise>().run = true;
        }

        //tilt the round up, up then down and stop the ride
        if (rideTime > 8f) 
        {
            if (angleOfRotation < 45f && !top) {
                transform.Rotate (rotationAxis, angle * Time.deltaTime);
                angleOfRotation += (rotationAxis.x * Time.deltaTime) * angle;
            } else if (!top) {
                angleOfRotation += (rotationAxis.x * Time.deltaTime) * angle;
            }
            
            if (angleOfRotation >= 90f && !top) {
                angleOfRotation = 45f;
                top = true;
            }
            
            if (angleOfRotation > 0f && top) {
                transform.Rotate (rotationAxis, -angle * Time.deltaTime);
                angleOfRotation -= (rotationAxis.x * Time.deltaTime) * angle;
            }

            if(rideTime > 80)
            {
                RotatingRide.GetComponent<RotateClockwise>().run = false;
            }

            if (rideTime > 85f) {
                top = false;
                rideTime = 0f;
            }
            
        }
        rideTime += 1f * Time.deltaTime;
    }
	
}
