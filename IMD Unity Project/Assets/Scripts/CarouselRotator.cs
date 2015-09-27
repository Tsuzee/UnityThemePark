using UnityEngine;
using System.Collections;

public class CarouselRotator : MonoBehaviour 
{
	private Vector3 rotationAxis = Vector3.up;
	private float rotationSpeed = 60.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (rotationAxis, rotationSpeed * Time.deltaTime);
	}
}
