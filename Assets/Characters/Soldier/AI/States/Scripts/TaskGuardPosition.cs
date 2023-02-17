using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGuardPosition : Node
{
    [SerializeField] Transform guardingPoint;
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
        if(transform.position != guardingPoint.position)
            { navMeshAgent.destination = guardingPoint.position; }

        lookingTimeCounter += Time.deltaTime;
        if(lookingTimeCounter >= timeToChangeLookingDirection)
        {
            transform.rotation = Quaternion.Euler(0f, lookingDirectionChangeAngle, 0f);
            lookingTimeCounter = 0f;
        }

        state = NodeState.RUNNING; 
        return state;
    }
}
