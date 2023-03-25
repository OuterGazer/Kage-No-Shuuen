using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;

public class TaskPatrol : Node
{
    [SerializeField] Transform patrolParent;
    public void SetPatrolParent(Transform patrolParent)
    {
        this.patrolParent = patrolParent;
    }
    [SerializeField] int startPatrolPointIndex = 0;
    [SerializeField] float reachThreshold = 0.5f;
    private float patrolSpeed;

    private NavMeshAgent navMeshAgent;

    private PatrolPoint[] patrolPoints;
    private int currentPatrolPointIndex;

    private void OnEnable()
    {
        //this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();
    }

    private void Start()
    {
        this.patrolPoints = patrolParent.GetComponentsInChildren<PatrolPoint>();

        patrolSpeed = ((SoldierBehaviour)belongingTree).PatrolSpeed;
        this.navMeshAgent = ((SoldierBehaviour) belongingTree).NavMeshAgent;
        currentPatrolPointIndex = startPatrolPointIndex;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.stoppingDistance = reachThreshold;

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
