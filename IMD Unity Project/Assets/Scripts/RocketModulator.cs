using UnityEngine;
using System.Collections;

public class RocketModulator : MonoBehaviour 
{
	private float frequency = 4.0f;
	private float magnitude = 1.0f;

	private float initialY;

	public bool redRocket = false;


	// Use this for initialization
	void Start () 
	{
		initialY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
        //move rockets up and down
		float nextY = initialY;
		if (redRocket) 
		{
			nextY = initialY + magnitude * Mathf.Sin (frequency * Time.time);
		} 
		else 
		{
			nextY = initialY + magnitude * Mathf.Cos (frequency * Time.time);
		}

		transform.position = new Vector3 (transform.position.x, nextY, transform.position.z);
	}
}
