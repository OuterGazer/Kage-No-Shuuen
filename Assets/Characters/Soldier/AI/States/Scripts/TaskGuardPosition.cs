using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGuardPosition : Node
{
    [SerializeField] Transform guardingPoint;
    public void SetGuard(Transform guardingParent)
    {
        guardingPoint = guardingParent;
    }
    [SerializeField] float timeToChangeLookingDirection = 5f;
    [SerializeField] float lookingDirectionChangeAngle = 45f;

    private NavMeshAgent navMeshAgent;

    private float lookingTimeCounter = 0f;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
    }

    public override NodeState Evaluate()
    {
        if (IsAgentFarAwayFromGuardingPoint())
        {
            navMeshAgent.destination = guardingPoint.position;

            state = NodeState.RUNNING;
            return state;
        }

        lookingTimeCounter += Time.deltaTime;
        if (lookingTimeCounter >= timeToChangeLookingDirection)
        {
            transform.rotation *= Quaternion.Euler(0f, lookingDirectionChangeAngle, 0f);
            lookingTimeCounter = 0f;
        }

        state = NodeState.RUNNING;
        return state;
    }

    private bool IsAgentFarAwayFromGuardingPoint()
    {
        return (guardingPoint.position - transform.position).sqrMagnitude > (navMeshAgent.stoppingDistance * navMeshAgent.stoppingDistance);
    }
}
