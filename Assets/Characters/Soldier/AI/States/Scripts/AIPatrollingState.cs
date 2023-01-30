using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPatrollingState : AIBaseState
{
    [SerializeField] Transform patrolParent;
    [SerializeField] int startPatrolPointIndex = 0;
    [SerializeField] float reachThreshold = 0.25f;

    PatrolPoint[] patrolPoints;
    int currentPatrolPointIndex;

    private NavMeshAgent navMeshAgent;

    protected override void InternalAwake()
    {
        base.InternalAwake();
        navMeshAgent = GetComponent<NavMeshAgent>();
        patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
    }

    protected override void InternalStart()
    {
        base.InternalStart();
        currentPatrolPointIndex = startPatrolPointIndex;
    }

    protected override void InternalUpdate()
    {

        base.InternalUpdate();

        navMeshAgent.destination = patrolPoints[currentPatrolPointIndex].transform.position;

        if ((navMeshAgent.destination - transform.position).sqrMagnitude < (reachThreshold * reachThreshold))
        {
            currentPatrolPointIndex++;

            if (currentPatrolPointIndex == patrolPoints.Length)
                currentPatrolPointIndex = 0;
        }

    }
}
