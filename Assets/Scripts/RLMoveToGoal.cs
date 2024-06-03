using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RLMoveToGoal : Agent
{
    [SerializeField] private Transform targetTransform;
    public float moveSpeed = 1f;
    private Vector3 startPosition;
    private bool isMoving = false;
    private Coroutine currentMoveCoroutine;
    private HashSet<Vector3> visitedPositions;

    public override void OnEpisodeBegin(){
        transform.position = new Vector3(1f,0.4f,1f);
        startPosition = transform.position;
        visitedPositions = new HashSet<Vector3>();

    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (isMoving) return; // Prevent new movement commands while already moving

        int move = actions.DiscreteActions[0];
        

        Vector3 moveDirection = Vector3.zero;

        if (move == 1) moveDirection += new Vector3(1f, 0f, 0f); // Move right
        if (move == 2) moveDirection += new Vector3(-1f, 0f, 0f); // Move left
        if (move == 3) moveDirection += new Vector3(0f, 0f, 1f); // Move forward
        if (move == 4) moveDirection += new Vector3(0f, 0f, -1f); // Move backward

        if (moveDirection != Vector3.zero)
        {
            currentMoveCoroutine = StartCoroutine(MoveToPosition(transform.position + moveDirection));
        }
    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        var discreteActions = actionOut.DiscreteActions;
        discreteActions[0] = 0; // No horizontal movement

        if (Input.GetKey(KeyCode.D)) discreteActions[0] = 1; // Move right
        if (Input.GetKey(KeyCode.A)) discreteActions[0] = 2; // Move left
        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 3; // Move forward
        if (Input.GetKey(KeyCode.S)) discreteActions[0] = 4; // Move backward
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Debug.Log("COROUTINE TargetPos"+targetPosition.x+" "+targetPosition.z);
        float elapsedTime = 0f;
        float duration = 1f / moveSpeed; // Time to move one cell

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
        if (visitedPositions.Contains(transform.position))
        {
            SetReward(-0.1f); // Penalize for revisiting
        }
        else
        {
            SetReward(1f); // Reward for new position
            visitedPositions.Add(transform.position);
        }
    }

    private void OnTriggerEnter(Collider other){
        
        if (other.CompareTag("Goal"))
        {
            if (currentMoveCoroutine != null)
            {
                Debug.Log("ONTRIGGER");
                StopCoroutine(currentMoveCoroutine);
                currentMoveCoroutine = null;
                isMoving = false;
            }
            Debug.Log("TRIGGER GOAL");
            SetReward(+1f);
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            if (currentMoveCoroutine != null)
            {
                Debug.Log("ONTRIGGER");
                StopCoroutine(currentMoveCoroutine);
                currentMoveCoroutine = null;
                isMoving = false;
            }
            Debug.Log("TRIGGER WALL");
            SetReward(-1f);
            EndEpisode();
        }
        
    }
}