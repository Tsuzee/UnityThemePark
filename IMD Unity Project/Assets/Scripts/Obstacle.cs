using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour 
{
	//change to private later, after finding correct value
	public float radius = 1.14f;

	public float Radius 
	{
		get { return radius;}
	}

	
	// Use this for initialization
	void Start () 
    {
        radius = GetComponent<Collider>().bounds.size.magnitude;
	}
}
