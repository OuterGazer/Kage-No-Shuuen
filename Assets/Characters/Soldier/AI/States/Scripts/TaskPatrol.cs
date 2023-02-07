using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskPatrol : Node
{
    [SerializeField] Transform patrolParent;
    [SerializeField] int startPatrolPointIndex = 0;
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float reachThreshold = 0.5f;

    private NavMeshAgent navMeshAgent;

    private PatrolPoint[] patrolPoints;
    private int currentPatrolPointIndex;

    private void Awake()
    {
        this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
    }

    private void Start()
    {
        this.navMeshAgent = ((SoldierRunnerBT) belongingTree).NavMeshAgent;
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
