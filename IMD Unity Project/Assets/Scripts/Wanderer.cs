using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wanderer : Vehicle 
{
    //get a reference to the GameManager
    private GameManager gm;

    //booleans
    public bool isSeekingFood = false;
    public bool isSeekingPlayer = false;

    //store target
    public GameObject myFoodTarget;
    public GameObject myCharTarget;
    public GameObject myEvadeTarget;

    //private array of game objects and others
    private GameObject[] obstacles;
    private List<GameObject> targetFlock;
    private List<GameObject> targetsEaten;
    private bool eatPlayer;
    
    
    //class weights
    public float seekWt = 50.0f;
    public float seekDist = 30.0f;
    public float maxDist = 100.0f;
    public float avoidWt = 10.0f;
    public float avoidDist = 1.0f;
    public float evadeWt = 10.0f;
    public float evadeDist = 20.0f;
    public float boundsWt = 100f;
    public float boundsRadius = 170f;
    public float wanderWt = 100f;

    /// <summary>
    /// Use for initialization
    /// </summary>
    override public void Start () 
    {
        base.Start();
        if (isWandering)
        {
            velocity = new Vector3(0,0,0);
        }
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        targetFlock = gm.GetComponent<GameManager>().Flock;
        myCharTarget = GameObject.FindGameObjectWithTag("MainPlayer");
        targetsEaten = new List<GameObject>();
    }
    
    /// <summary>
    /// Update
    /// </summary>
    void Update () 
    {
        base.Update();

    }//end update

    /// <summary>
    /// calculate the steering forces
    /// </summary>
    protected override void CalcSteeringForces()
    {
        targetFlock = gm.GetComponent<GameManager>().Flock;
        Vector3 force = Vector3.zero;
        float distToFood = 1000000;
        float distToPlayer = (transform.position - myCharTarget.transform.position).magnitude;
        isWandering = true;

        //check to see if something yummy to eat is near
        if (targetFlock != null)
        {
            foreach (GameObject food in targetFlock)
            {
                //get the distance between the food and me
                float dist = Mathf.Abs((transform.position - food.transform.position).magnitude);
                if (dist < distToFood)
                {
                    distToFood = dist;
                }

                //if the food is to far away stop checking the flock to save calls
                if (distToFood > maxDist)
                {
                    break;
                }

                //if food is close set it as a target
                myFoodTarget = food;
                isSeekingFood = true;
                isSeekingPlayer = false;
            }
        }

        if (distToPlayer < distToFood)
        {
            distToFood = distToPlayer;
            isSeekingFood = false;
            isSeekingPlayer = true;
        }

        //if the target is close enough, seek it to eat it
        if(distToFood <= seekDist)
        {
            isWandering = false;
            if(isSeekingFood)
            {
                force += Seek(myFoodTarget.transform.position) * seekWt;
            }
            else if(isSeekingPlayer)
            {
                force += Seek(myCharTarget.transform.position) * seekWt;
            }

        }

        if (distToFood < 1.3f)
        {
            if(isSeekingFood)
            {
                targetsEaten.Add(myFoodTarget);
            }
            else if(isSeekingPlayer)
            {
                //fade screen for character death and move to respawn
                gm.GetComponent<GameManager>().FadeScreen = true;
                gm.GetComponent<GameManager>().player = myCharTarget;
                isSeekingPlayer = false;
            }
        }

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
      
        if (myEvadeTarget != null)
        {
            if ((transform.position - myEvadeTarget.transform.position).magnitude < evadeDist)
            {
                force += Evade(myEvadeTarget.transform.position) * evadeWt;
            }
        }
        
        force += boundsWt * StayInBounds(boundsRadius, gm.Center.transform.position);
        
        
        force.y = 0;

        //check to see if our guy is flying
        if (transform.position.y > 1.09f)
        {
            transform.position = new Vector3(transform.position.x, 1.08f, transform.position.z);
        }

        ApplyForce(force);


        if (targetsEaten.Count > 0)
        {
            foreach(GameObject eaten in targetsEaten)
            {
                gm.GetComponent<GameManager>().modFlock.Add(eaten);
                targetsEaten.Remove(eaten);
            }
            targetsEaten.Clear();
        }
        
        //draw debug lines
        //Debug.DrawLine(transform.position, transform.position + force, Color.blue);
        //Debug.DrawLine(transform.position, myTarget.transform.position, Color.red);
    }//end calc steering
}//end class
