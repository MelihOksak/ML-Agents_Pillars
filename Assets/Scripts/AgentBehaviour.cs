using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerMovement : Agent{

    [SerializeField] private Transform targetTransform1;
    [SerializeField] private Transform targetTransform2;
    [SerializeField] private Transform targetTransform3;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private const float jumpForce = 5.0f;
    private Rigidbody rb;
    private bool isJumping=false;

    public override void OnEpisodeBegin() {
        float xpos=UnityEngine.Random.Range(-2f,2f);
        float zpos = UnityEngine.Random.Range(-1.8f, 1.8f);

        transform.SetLocalPositionAndRotation(new Vector3(xpos,0,zpos), Quaternion.identity);
        rb = GetComponent<Rigidbody>();       
        
        targetTransform1.gameObject.SetActive(true);
        targetTransform2.gameObject.SetActive(true);
        targetTransform3.gameObject.SetActive(true);
        SetReward(0f);
        
      

    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform1.localPosition);
        sensor.AddObservation(targetTransform2.localPosition);
        sensor.AddObservation(targetTransform3.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
       
        float speed = 2f;
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        int jump= actions.DiscreteActions[0];
        transform.localPosition -= speed * Time.deltaTime * new Vector3(moveX, 0, moveZ);
        if (jump == 1 && isJumping==false){
            
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
        AddReward(-0.0001f);



    }

    public override void Heuristic(in ActionBuffers actionsOut) 
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        discreteActions[0] = (int)Input.GetAxisRaw("Jump");
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            float reward= goal.getScore();
            Debug.Log(reward);
            other.gameObject.SetActive(false);
            AddReward(reward);
            if (reward > 20)
            {
                floorMeshRenderer.material = winMaterial;
                EndEpisode();
            }
            
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.05f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
            Debug.Log("oob");

        }

    }
    private void OnCollisionEnter(Collision collision) {
        isJumping = false;
    }

}
