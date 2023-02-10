using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskPatrol : Node
{
    [SerializeField] Transform patrolParent;
    [SerializeField] int startPatrolPointIndex = 0;
    private float reachThreshold;
    private float patrolSpeed;

    private NavMeshAgent navMeshAgent;

    private PatrolPoint[] patrolPoints;
    private int currentPatrolPointIndex;

    private void Awake()
    {
        this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
    }

    private void Start()
    {
        patrolSpeed = ((SoldierRunnerBT)belongingTree).PatrolSpeed;
        this.navMeshAgent = ((SoldierRunnerBT) belongingTree).NavMeshAgent;
        reachThreshold = navMeshAgent.stoppingDistance;
        currentPatrolPointIndex = startPatrolPointIndex;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = patrolSpeed;

        navMeshAgent.destination = patrolPoints[currentPatrolPointIndex].transform.position;

        if ((navMeshAgent.destination - transform.position).sqrMagnitude < (reachThreshold * reachThreshold))
        {
            currentPatrolPointIndex++;

            if (currentPatrolPointIndex == patrolPoints.Length)
                currentPatrolPointIndex = 0;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
