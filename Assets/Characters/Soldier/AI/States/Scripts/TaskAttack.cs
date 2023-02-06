using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskAttack : Node
{
    private NavMeshAgent navMeshAgent;

    private float attackRatePerSecond = 1f;
    private float attackCounter = 0.5f;

    public TaskAttack() 
    {
        navMeshAgent = SoldierRunnerBT.NavMeshAgent;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = 0f;

        Transform target = (Transform) GetData("target");

        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        attackCounter += Time.deltaTime;
        if(attackCounter > attackRatePerSecond)
        {
            SoldierRunnerBT.CharacterAnimator.PlaySlashAnimation();
            attackCounter = 0f;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
