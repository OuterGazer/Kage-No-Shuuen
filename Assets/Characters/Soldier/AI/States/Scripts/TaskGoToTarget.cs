using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private void Start()
    {
        navMeshAgent = SoldierRunnerBT.NavMeshAgent;
        decisionMaker = SoldierRunnerBT.DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDisable()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    //public TaskGoToTarget()
    //{
    //    navMeshAgent = SoldierRunnerBT.NavMeshAgent;
    //    decisionMaker = SoldierRunnerBT.DecisionMaker;
    //    decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    //}

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = SoldierRunnerBT.RunningSpeed;

        Transform target = (Transform)GetData("target");

        if (target)
        {
            navMeshAgent.destination = target.position;
            state = NodeState.RUNNING;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }
        
    }

    private void EraseInterestingTarget()
    {
        //if (!SoldierRunnerBT.IsTargetInAttackRange)
        if(state == NodeState.RUNNING)
        { ClearData("target"); }
    }
}
