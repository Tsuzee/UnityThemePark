using UnityEngine;
using System.Collections;

public class RotateCounterclockwise : MonoBehaviour 
{

    private Vector3 rotationAxis = Vector3.up;
    private float rotationSpeed = 120.0f;
    
    // Use this for initialization
    void Start () 
    {
        
    }
    
    // Update is called once per frame
    void Update () 
    {
        //cause an object to rotate counter clockwise
        transform.Rotate (rotationAxis, -(rotationSpeed * Time.deltaTime));
    }
}
