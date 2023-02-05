using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskPatrol : Node
{
    private Transform transform;
    private NavMeshAgent navMeshAgent;

    private PatrolPoint[] patrolPoints;
    private int startPatrolPointIndex = 0;
    private int currentPatrolPointIndex;
    private float reachThreshold = 0.5f;

    public TaskPatrol(Transform patrolParent) 
    {
        this.navMeshAgent = SoldierRunnerBT.NavMeshAgent;
        transform = this.navMeshAgent.transform;

        this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
        currentPatrolPointIndex = startPatrolPointIndex;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = SoldierRunnerBT.PatrolSpeed;

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
