using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Leader : Vehicle 
{
    //get a reference to the GameManager
    private GameManager gm;

    
    //private array of game objects and others
    private GameObject[] obstacles;
    
    
    //class weights
    public float seekWt = 50.0f;
    public float seekDist = 30.0f;
    public float maxDist = 100.0f;
    public float avoidWt = 10.0f;
    public float avoidDist = 1.0f;
    public float boundsWt = 100f;
    public float boundsRadius = 170f;
    public float wanderWt = 100f;
    
    
    // Use this for initialization
    override public void Start () 
    {
        base.Start();
        if (isWandering)
        {
            velocity = new Vector3(0,0,0);
        }
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    // Update is called once per frame
    void Update () 
    {
        base.Update();
        
    }

    /// <summary>
    /// calculate steering forces
    /// </summary>
    protected override void CalcSteeringForces()
    {
        Vector3 force = Vector3.zero;

        isWandering = true;

        
        if (isWandering && (Time.frameCount % 120) == 0)
        {
            force += Wander() * wanderWt;
        }
        
        force.y = 0;
        //force = Vector3.ClampMagnitude(force, maxforce);
        
        
        //could add other steering forces here
        foreach (GameObject obsta in obstacles)
        {
            
            force += AvoidObstacle(obsta, avoidDist) * avoidWt;
        }
        
        force += boundsWt * StayInBounds(boundsRadius, gm.Center.transform.position);
        
        
        force.y = 0;

        //check to see if our guy is flying
        if (transform.position.y > 0.1f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
        
        ApplyForce(force);
    }
}
