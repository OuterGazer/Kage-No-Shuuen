using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private Transform transform;
    private NavMeshAgent navMeshAgent;

    private float attackThreshold = 1.5f;

    public TaskGoToTarget()
    {
        this.navMeshAgent = SoldierRunnerBT.NavMeshAgent;
        transform = navMeshAgent.transform;

        this.navMeshAgent.speed = SoldierRunnerBT.RunningSpeed;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (((navMeshAgent.destination - transform.position).sqrMagnitude < (attackThreshold * attackThreshold)))
        { navMeshAgent.destination = target.position; }

        state = NodeState.RUNNING;
        return state;
    }
}
