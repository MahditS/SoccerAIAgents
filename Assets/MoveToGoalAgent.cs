using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;
public class MoveToGoalAgent : Agent{
    
    [SerializeField] private Transform target;
    [SerializeField] private Transform goal;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform BRPost;
    [SerializeField] private Transform BLPost;
    [SerializeField] private Transform FRPost;
    [SerializeField] private Transform EBRPost;
    [SerializeField] private Transform EBLPost;
    [SerializeField] private Transform EFRPost;
    [SerializeField] private Vector3 offset;

    [SerializeField] private Text redScore;
    [SerializeField] private Text blueScore;
    
    [SerializeField] private Rigidbody targetRB;

    [SerializeField] private Rigidbody rb;
    public int score = 0;
    public int bscore = 0;

    private Vector3 distance = Vector3.zero;
    public bool isRed;


    public override void OnEpisodeBegin()
    {
        goal.localPosition = new Vector3(-7.744f, 1.021548f, -5.72f);
        // transform.localPosition = new Vector3(17.12f, 1.311058f, -6.58f) + offset;
        target.localPosition = new Vector3(17.3f, -0.2215478f, -0.68f);
        // target.localPosition = new Vector3(9.39f, -0.2215478f, -0.71f);
        targetRB.velocity = Vector3.zero;
        targetRB.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Vector3 targetPos = target.position;
        // goal.localPosition = new Vector3(-7.744f, 1.021548f, Random.Range(-13.74f, 2.02f));
        // target.position = targetPos;

        //OUTDATED
        // target.localPosition = new Vector3(Random.Range(-3.77f, 3.71f), 0.8f, Random.Range(-10.4f, -2.93f));

        // target.position = new Vector3(Random.Range(-9.05f, 8.85f), 1.95f, Random.Range(-8.918f, 8.514f));
        // target.position = new Vector3(Random.Range(-2.09f, 2.17f), 1.95f, Random.Range(-13.684f, 1.253f));
        // PLAT SIZE X 19.01 Y 1 Z 18.9 
        //NEW PLAT SIZE X 16.45628 Y 0.47674 Z 24.04512
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition - target.localPosition);
        sensor.AddObservation(goal.localPosition);
        sensor.AddObservation(target.localPosition - goal.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions){
     int x = actions.DiscreteActions[0];
     int z = actions.DiscreteActions[1];

     Vector3 addForce = new Vector3(0,0,0);
     
     switch(x){
        case 0: addForce.x = 0; break;
        case 1: addForce.x = -1f; break;
        case 2: addForce.x = +1f; break;
     }
     switch(z){
        case 0: addForce.z = 0; break;
        case 1: addForce.z = -1f; break;
        case 2: addForce.z = +1f; break;
     }
     rb.velocity = addForce * moveSpeed * Time.deltaTime + new Vector3(0,rb.velocity.y,0);
    //  transform.localPosition += new Vector3(x,0,z) * Time.deltaTime * moveSpeed;
    // rb.AddForce(addForce * Time.deltaTime * moveSpeed);
    //  Debug.Log(BRPost.position);
    if(isRed)
    {
     if(target.localPosition.z < BRPost.localPosition.z && target.localPosition.z > BLPost.localPosition.z && target.localPosition.x > BRPost.localPosition.x && target.localPosition.x < FRPost.localPosition.x)
     {  
        Debug.Log("Score Red!");
        score++;
        redScore.text = score.ToString();
        SetReward(+1f);
        EndEpisode();
     }
     if(target.localPosition.z > EBRPost.localPosition.z && target.localPosition.z < EBLPost.localPosition.z && target.localPosition.x < EBRPost.localPosition.x && target.localPosition.x > EFRPost.localPosition.x)
     {  
        Debug.Log("Scored On Red!");
        bscore++;
        blueScore.text = bscore.ToString();
        SetReward(-1f);
        EndEpisode();
     }
    }
    else
    {
    if(target.localPosition.z < BRPost.localPosition.z && target.localPosition.z > BLPost.localPosition.z && target.localPosition.x > BRPost.localPosition.x && target.localPosition.x < FRPost.localPosition.x)
     {  
        Debug.Log("Scored On Blue!");
        score++;
        redScore.text = score.ToString();
        SetReward(-1f);
        EndEpisode();
     }
     if(target.localPosition.z > EBRPost.localPosition.z && target.localPosition.z < EBLPost.localPosition.z && target.localPosition.x < EBRPost.localPosition.x && target.localPosition.x > EFRPost.localPosition.x)
     {  
        Debug.Log("Score Blue!");
        bscore++;
        blueScore.text = bscore.ToString();
        SetReward(+1f);
        EndEpisode();
     }
    }
     if(transform.localPosition.y < -5f)
     {
        SetReward(-1f);
         EndEpisode();
     }
     if(target.localPosition.y < -5f)
     {
        SetReward(-1f);
        EndEpisode();
     }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        int xc = (int)Mathf.Round(Input.GetAxisRaw("Horizontal"));
        int zc = (int)Mathf.Round(Input.GetAxisRaw("Vertical"));
        switch(zc)
        {
            case 0: discreteActions[0] = 0; break;
            case -1: discreteActions[0] = 1; break;
            case 1: discreteActions[0] = 2; break;
        }
        switch(xc)
        {
            case 0: discreteActions[1] = 0; break;
            case -1: discreteActions[1] = 1; break;
            case 1: discreteActions[1] = 2; break;
        }

    }

    public void OnCollisionEnter(Collision collider)
    {
        if(collider.gameObject.name == "Goal")
        {
            Debug.Log("Hit");
            SetReward(+0.025f);
            // EndEpisode();
        }
        else
        {
            SetReward(-0.03f);
        }
    }
}
