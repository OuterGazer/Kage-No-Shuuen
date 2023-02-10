using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskSearchTarget : Node
{
    [SerializeField] int totalSearchSpots = 5;
    [SerializeField] float timeToChangeSearchSpot = 1f;
    [SerializeField] float searchRadius = 3f;
    private float patrolSpeed;
    private float searchCounter = 0f;
    private float changeSearchSpotCounter = 0;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        patrolSpeed = ((SoldierRunnerBT)belongingTree).PatrolSpeed;
        navMeshAgent = ((SoldierRunnerBT)belongingTree).NavMeshAgent;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = patrolSpeed;

        Transform s = (Transform)GetData("searchTarget");

        if (!s)
        {
            state = NodeState.FAILURE;
            return state;
        }

        searchCounter += Time.deltaTime;
        if(changeSearchSpotCounter >= totalSearchSpots) 
        {
            changeSearchSpotCounter = 0;
            searchCounter = 0f;
            ClearData("searchTarget"); 
        }
        else if(searchCounter >= timeToChangeSearchSpot)
        {
            Vector3 lastSeenSposition = new Vector3(s.position.x, 0f, s.position.z);
            Vector2 searchTargetPosition = searchRadius * Random.insideUnitCircle;
            float posX = lastSeenSposition.x + searchTargetPosition.x;
            float posY = lastSeenSposition.z + searchTargetPosition.y;
            navMeshAgent.destination = new Vector3(posX, 0f, posY);

            changeSearchSpotCounter++;
            searchCounter = 0f;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
