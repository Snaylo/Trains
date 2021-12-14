using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrunkAgent : Agent
{

    private Transform target, startingPoint;
    private Animator anim;
    public float speed = 5.0f;
    private CharacterController cc;
    private float inputX = 0.0f, inputY = 0.0f;
    private Vector3 lastPosition = Vector3.zero;
    private float previousDistanceToTarget = 500.0f;
    
    public override void Initialize()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        target = GameObject.FindWithTag("Target").transform;
        startingPoint = GameObject.FindWithTag("Start").transform;
    }

    private void Reset()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color =
            new Color(Random.value, Random.value, Random.value);
        var bounds = startingPoint.GetComponent<BoxCollider>().bounds;
        var start = new Vector3(Random.Range(bounds.min.x, bounds.max.x), 0.0f, 
            Random.Range(bounds.min.z, bounds.max.z));
        transform.localPosition = start;
    }
    
    public override void OnEpisodeBegin()
    {
        Reset();
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var cont = actions.ContinuousActions;
        inputX = cont[0];
        inputY = cont[1]; 

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Vector3.Distance(transform.position, target.position));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actionBuffers = actionsOut.ContinuousActions;
        inputX = Input.GetAxis ("Horizontal");
        actionBuffers[0] = inputX;
        inputY = Input.GetAxis ("Vertical");
        actionBuffers[1] = inputY;
    }
    

    private void Update()
    {   print(Vector3.Distance(transform.position, target.position));
        if(Vector3.Distance(transform.position, target.position) < previousDistanceToTarget) AddReward(Map(Vector3.Distance(transform.position, target.position), 0, 250, 0.1f, 0));
        else AddReward(Map(Vector3.Distance(transform.position, target.position), 0, 250, 0, -0.1f));
            /*AddReward(Vector3.Distance(transform.position, target.position) < previousDistanceToTarget
                ? Map(Vector3.Distance(transform.position, target.position), 0, 250, 0.1f, 0)
                : Map(Vector3.Distance(transform.position, target.position), 0, 250, 0, -0.1f));*/
        cc.Move(new Vector3(inputY * -speed, 0, inputX * speed));
        anim.SetFloat("Speed", inputY);
        //if(Vector3.Distance(lastPosition,transform.position)<0.01f) {AddReward(-0.1f);}
        lastPosition = transform.localPosition;
        previousDistanceToTarget = Vector3.Distance(transform.position, target.position);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 11) {
            AddReward(5.0f);
            print("Cumulative reward is " + GetCumulativeReward());
            EndEpisode(); //reached goal
}
        else if (other.gameObject.layer == 8) {
            AddReward(-1.0f);
            print("Cumulative reward is " + GetCumulativeReward());
            EndEpisode(); // Hit by Train 
}
        /*else if (other.gameObject.layer == 14)
        {
            AddReward(0.5f);
            print("Cumulative reward is " + GetCumulativeReward());
        }*/
    }
    
    public float Map(float value, float min1, float max1, float min2, float max2) {
        return min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
    }
}
