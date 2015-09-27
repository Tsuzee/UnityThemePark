using UnityEngine;
using System.Collections;

public class RotateClockwise : MonoBehaviour 
{

	// Use this for initialization
    private Vector3 rotationAxis = Vector3.up;
    private float rotationSpeed = 60.0f;
    public bool run = false;
    
    // Use this for initialization
    void Start () 
    {
    }
    
    // Update is called once per frame
    void Update () 
    {
        if (run)
        {
            //cause an object to rotate clockwise
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}
