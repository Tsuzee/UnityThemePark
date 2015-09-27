using UnityEngine;
using System.Collections;

public class Seeker : Vehicle 
{
    //get a reference to the GameManager
    private GameManager gm;

    //store target
    public GameObject myTarget;
    public GameObject myLeader;
    private GameObject myCharTarget;

    //private array of game objects
    private GameObject[] obstacles;


    //class weights
    public float seekWt = 50.0f;
    public float avoidWt = 10.0f;
    public float avoidDist = 1.0f;
    public float fleeWt = 250f;
    public float separationWt = 20f;
    public float separationDist = 2f;
    public float cohesionWt = 1f;
    public float alignmentWt = 20f;
    public float boundsWt = 100f;
    public float arrivalWt = 20f;

    private bool hasArrived = false;

	// Use this for initialization
	override public void Start () 
    {
        base.Start();
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        myCharTarget = GameObject.FindGameObjectWithTag("MainPlayer");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myTarget = null;
	}
	
	// Update is called once per frame
	void Update () 
    {
        base.Update();

        //check to see if our guy is flying
        if (transform.position.y > 1.09f)
        {
            transform.position = new Vector3(transform.position.x, 1.08f, transform.position.z);
        }
	}

	protected override void CalcSteeringForces()
	{
        Vector3 force = Vector3.zero;
        
        if (myTarget != null)
        {
            force += Seek(myTarget.transform.position) * seekWt;
            force.y = 0;
        }

        if (myLeader != null)
        {
            Vector3  posDif = myLeader.transform.position - transform.position;

            Vector3 posBehind = myLeader.transform.forward;
            posBehind = posBehind.normalized * 3;
            posBehind *= -1;

            posBehind += myLeader.transform.position;
            posBehind.y = 0;
            force += Arrival(posBehind, (posDif).magnitude) * arrivalWt;

            if(Mathf.Abs((posDif).magnitude) < 3)
            {
                hasArrived = true;
            }
            else
            {
                hasArrived = false;
            }
        }
        
        //force = Vector3.ClampMagnitude(force, maxforce);


        //could add other steering forces here
        foreach (GameObject obsta in obstacles)
        {

            force += AvoidObstacle(obsta, avoidDist) * avoidWt;
        }

        force += alignmentWt * Alignment (gm.FlockDirection);

        force += cohesionWt * Cohesion(gm.Centroid);

        if(hasArrived)
        {
            force = new Vector3(0,0,0);
            velocity = new Vector3(0,0,0);
        }

        force += separationWt * Separation (gm.Flock, separationDist);

        if ((myCharTarget.transform.position - transform.position).magnitude < 3)
        {
            Debug.Log((myCharTarget.transform.position - transform.position).magnitude);
            force += Flee(myCharTarget.transform.position)  * fleeWt;
        }

        //force += boundsWt * StayInBounds (27, Vector3.zero);

        force = Vector3.ClampMagnitude(force, maxforce);

        force.y = 0;

        ApplyForce(force);

        //draw debug lines
        //Debug.DrawLine(transform.position, transform.position + force, Color.blue);
        //Debug.DrawLine(transform.position, myTarget.transform.position, Color.red);
	}
}
