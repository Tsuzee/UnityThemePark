using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Automatically adds a CharacterController component to the same GameObject this script is on
	
//Reminds developer to not delete the CharController comp. 
	
	
[RequireComponent(typeof(CharacterController))]
	
abstract public class Vehicle : MonoBehaviour 
{
    //booleans
    public bool isWandering;

	//fields the Vehicle class needs - similar to Vehicles in Processing
	public float maxSpeed = 6.0f;
    private float unAdjMaxSpeed;
	public float maxforce = 12.0f;
	public float mass = 1.0f;
	public float radius = 1.0f;
	public float gravity = 20.0f;
		
	//used for movement
	protected Vector3 velocity;
	protected Vector3 acceleration;
	protected Vector3 desiredVelocity;
    protected Vector3 fwd;
    protected Vector3 right;

    //private variables
    private Vector3 lastPos;
		
	//property for velocity
	public Vector3 Velocity 
    {
		get { return velocity; }
		set { velocity = value; }
	}
		
	//internal reference to the CharacterController component
	protected CharacterController charControl;	

	abstract protected void CalcSteeringForces();

	// Use this for initialization
	public virtual void Start () 
	{
		acceleration = Vector3.zero;
		velocity = transform.forward;
		charControl = gameObject.GetComponent<CharacterController>();
        lastPos = transform.position;
        unAdjMaxSpeed = maxSpeed;
	}
		

    ///<summary>
    /// Update is called once per frame, at X frames per second
    /// </summary>	
	protected void Update () 
	{
        CalcSteeringForces();

		//update velocity
		velocity += acceleration * Time.deltaTime;
        //Debug.Log("Accel in Update " + acceleration);

		//later we can reflect this to reflect terrian
		//velocity.y = 0;
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //orient the transform to face where I'm going
		if (velocity != Vector3.zero) 
        {
			transform.forward = velocity.normalized;
		}

        //CharacterController will do the moving
        charControl.Move(velocity * Time.deltaTime);

        //velocity debug line
        Vector3 test3 = transform.position - velocity;
        test3.y = transform.position.y;

        //Is my vehicle stuck??
        if (Mathf.Abs(velocity.magnitude) > 0 && lastPos == transform.position)
        {
            //Debug.Log("I'm Stuck");
            Stuck();
        }

        //zero acceleration, update lastPos
        acceleration = Vector3.zero;
        lastPos = transform.position;

        //track the forward and right vectors
        fwd = transform.forward;
        fwd.Normalize();
        //fwd.y = 0;
        right.x = fwd.z;
        right.z = -fwd.x;
        //fwd *= -1;

        //debug lines for fwd/right wectors
        Vector3 test1 = transform.position + (fwd * 5f);
        test1.y = transform.position.y;
        Vector3 test2 = transform.position + (right * 5f);
        test2.y = transform.position.y;
        //Vector3 test3 = transform.position - (velocity);
        //test3.y = transform.position.y;
        //Debug.DrawLine(transform.position, test1, Color.yellow);
        //Debug.DrawLine(transform.position, test2, Color.green);
        //Debug.DrawLine(transform.position, test3, Color.white);

	}//end update

    void LateUpdate() {
        //transform.position.Set(transform.position.y, Terrain.activeTerrain.SampleHeight(transform.position), transform.position.z);
    }


    ///<summary>
    /// apply the force to acceleration based on mass
    /// </summary>
	protected void ApplyForce(Vector3 steeringForce)
	{
        acceleration += steeringForce / mass;
	}//end apply force

    ///<summary>
    /// calculate seek force for a designated target
    /// </summary>
	protected Vector3 Seek(Vector3 targetPosition)
	{
        //find desired velocity, scale by speed
        desiredVelocity = targetPosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        desiredVelocity -= velocity;

        //Debug.Log("Seek Force " + desiredVelocity);

		return desiredVelocity;
	}//end seek

    ///<summary>
    /// calculate obstacle avoidance force
    /// </summary>
	protected Vector3 AvoidObstacle(GameObject obst, float safeDistance)
	{
        Vector3 steerForce = Vector3.zero;
        float fwdDot;
        float rightDot;
        float totalRadius = (obst.GetComponent<Collider>().bounds.size.magnitude / 2) + radius;
        
        // Create vecToCenter - a vector from the character to the center of the obstacle
        Vector3 VtoC = Vector3.zero;
        VtoC = obst.transform.position;
        VtoC -= transform.position;
        
        //fwdDot = Vector3.Dot(fwd, VtoC);
        //rightDot = Vector3.Dot(right, VtoC);
        fwdDot = Vector3.Dot(VtoC, fwd);
        rightDot = Vector3.Dot(VtoC, right);
        
        //Find the distance to the obstacle and Return a zero vector if the obstacle is too far to concern us
        //if( (obst.radius/2 + vehicle.r + safeDistance) < VtoC.mag() )
        if( Mathf.Abs(totalRadius + safeDistance) < Mathf.Abs(VtoC.magnitude) )
        {
            return steerForce;
        }
        
        // Return a zero vector if the obstacle is behind us
        // (dot product of vecToCenter and forward is negative)
        if( fwdDot < 0 )
        {
            return steerForce;
        }
        
        // Use the dot product of the vector to obstacle center (vecToCenter) and the unit vector
        // to the right (right) of the vehicle to find the distance between the centers
        // of the vehicle and the obstacle
        // Compare this to the sum of the radii and return a zero vector if we can pass safely
        if( rightDot > Mathf.Abs(totalRadius + safeDistance/2) )
        {
            return steerForce;
        }
        
        Debug.DrawLine(transform.position, obst.transform.position, Color.magenta, 4f);
        // If we get this far we are on a collision course and must steer
        // Use the sign of the dot product between the vector to center (vecToCenter) and the
        // vector to the right (right) to determine whether to steer left or right
        if(rightDot > 0)
        {
            //steerForce = right * (-maxSpeed);
            steerForce = transform.right * -unAdjMaxSpeed;
        }
        else
        {
            //steerForce = right * maxSpeed;
            steerForce = transform.right * unAdjMaxSpeed;
        }
        
        
        // For each case calculate desired velocity using the right vector and maxSpeed
        // Compute the force required to change current velocity to desired velocity
        // Consider multiplying this force by safeDistance/dist to increase the relative weight
        // of the steering force when obstacles are closer.
        //steer.mult(safeDistance / VtoC.mag());
        
        steerForce *= VtoC.magnitude;
        //Debug.DrawLine(transform.position, (transform.position - steerForce), Color.black, 2f);
        
        //anti flight prevention measures
        steerForce.y = 0;


        return steerForce;
	}//end Avoid Obstacle


    ///<summary>
    /// Will help a vehicle become unstuck from an object
    /// </summary>
    protected void Stuck()
    {
        //hijack the characters movement here temporally to get it unstuck
    }//end stuck

    ///<summary>
    /// Keeps flockers from clumping up together
    /// </summary>
    protected Vector3 Separation(List<GameObject> objList, float separationDistance)
    {
        Vector3 total = Vector3.zero;
        foreach (GameObject f in objList) {
            Vector3 dv = transform.position - f.transform.position;
            float dist = dv.magnitude;
            
            if(dist > 0 && dist < separationDistance)
            {
                dv *= separationDistance / dist;
                dv.y = 0;
                total += dv;
            }//end if
        }//end foreach
        
        total = total.normalized * maxSpeed;
        total -= velocity;
        
        return total;
    }//end separation

    ///<summary>
    /// Keeps flockers headed in the same general direction
    /// </summary>
    protected Vector3 Alignment(Vector3 alignVector)
    {
        Vector3 dv = alignVector.normalized * maxSpeed;
        dv -= velocity;
        dv.y = 0;
        
        return dv;
    }//end alignment

    ///<summary>
    /// Keeps flockers together
    /// </summary>
    protected Vector3 Cohesion(Vector3 cohesionVector)
    {
        return Seek (cohesionVector);
    }//end cohesion

    ///<summary>
    /// Sets a point for vehicles to seek so they stay within a bounded area
    /// </summary>
    public Vector3 StayInBounds(float radius, Vector3 center)
    {
        if ( Mathf.Abs(transform.position.x - center.x) > radius || Mathf.Abs(transform.position.z - center.z) > radius )
        {
            return Seek (center);
        }
        
        return Vector3.zero;
    }//end Stay in Bounds

    ///<summary>
    /// Wandering function to make the vehicle go around aimlessly
    /// </summary>
    protected Vector3 Wander()
    {
        //temp vectors
        Vector3 centerOfTheCircle = fwd;
        Vector3 displacement = Vector3.zero;

        //get a random vector from a circle around the vehicle with a radius of 2
        //used a Vector2 because I only need two coordinates on a flat plane
        Vector2 temp = Random.insideUnitCircle * 2;
        displacement.x = temp.x;
        displacement.z = temp.y;

        //create a new vector from the center to our random point
        displacement -= centerOfTheCircle;

        //add the two vectors together and return
        centerOfTheCircle += displacement;

        Debug.DrawLine( transform.position, transform.position + centerOfTheCircle, Color.yellow, 10f);

        return centerOfTheCircle;
    }//end wander

    ///<summary>
    /// Arrive at a location and wait there
    /// </summary>
    protected Vector3 Arrival(Vector3 targetPosition, float distToTarget)
    {
        Vector3 desiredVelocity = Seek(targetPosition);

        if (distToTarget < 10f) 
        {
            //slowing down
            desiredVelocity *= (distToTarget / 10f);
        }

        desiredVelocity -= velocity;

        return desiredVelocity;
    }//end arrival

    ///<summary>
    /// Flee a target
    /// </summary>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        //find desired velocity, scale by speed
        desiredVelocity = targetPosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        desiredVelocity -= velocity;
        desiredVelocity *= -1;
        return desiredVelocity;
    }//end flee

    ///<summary>
    /// Evand a target
    /// </summary>
    protected Vector3 Evade(Vector3 target)
    {
        Vector3 toTheTarget = target - transform.position;
        float distance = toTheTarget.magnitude;
        float Tnum = distance / maxSpeed;
        Vector3 targetPosition = target + velocity * Tnum;
        Debug.DrawLine (transform.position, targetPosition, Color.grey);
        return Flee (targetPosition);
    }
}//end Class
