using UnityEngine;
using System.Collections;

public class CoasterCar : Vehicle 
{
    GameObject me;

    //path to follow
    public GameObject[] trackPath;


    //class weights
    public float seekWt = 10.0f;
    public int nodeNum = 0;

    
    // Use this for initialization
    override public void Start () 
    {
        base.Start();
    }
    
    // Update is called once per frame
    void Update () 
    {
        base.Update();
        
    }
    
    protected override void CalcSteeringForces()
    {
        Vector3 force = Vector3.zero;
        me = GameObject.FindGameObjectWithTag("Front of Car");
        force += Seek(trackPath [nodeNum].transform.position) * seekWt;


        float dist = (me.transform.position - trackPath [nodeNum].transform.position).magnitude;

        if (dist < 0.5f)
        {
            nodeNum++;
            if(nodeNum > trackPath.Length - 1)
            {
                nodeNum = 0;
            }
        }

        ApplyForce(force);
        
        //draw debug lines
        //Debug.DrawLine(transform.position, transform.position + force, Color.blue);
        //Debug.DrawLine(transform.position, myTarget.transform.position, Color.red);
    }
}
