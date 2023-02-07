using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskPatrol : Node
{
    [SerializeField] Transform patrolParent;
    [SerializeField] float patrolSpeed = 2f;

    private NavMeshAgent navMeshAgent;

    private PatrolPoint[] patrolPoints;
    private int startPatrolPointIndex = 0;
    private int currentPatrolPointIndex;
    private float reachThreshold = 0.5f;

    private void Start()
    {
        this.navMeshAgent = ((SoldierRunnerBT) belongingTree).NavMeshAgent;

        this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
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
