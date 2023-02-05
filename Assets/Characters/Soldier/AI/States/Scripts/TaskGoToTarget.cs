using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private Transform transform;
    private NavMeshAgent navMeshAgent;

    private float attackThreshold = 3f;

    public TaskGoToTarget()
    {
        navMeshAgent = SoldierRunnerBT.NavMeshAgent;
        transform = navMeshAgent.transform;
    }


    // TODO: speed is not changing upon entering this node and somehow the character "adds" waypoints to a queue and doesn't go to the player directly upon sight
    public override NodeState Evaluate()
    {
        navMeshAgent.speed = SoldierRunnerBT.RunningSpeed;

        Transform target = (Transform)GetData("target");

        if (target)
        {
            if (((target.position - transform.position).sqrMagnitude > (attackThreshold * attackThreshold)))
            { navMeshAgent.destination = target.position; }

            state = NodeState.RUNNING;
            return state;
        }
        else  
        {
            state = NodeState.FAILURE;
            return state;
        }
        
    }
}
