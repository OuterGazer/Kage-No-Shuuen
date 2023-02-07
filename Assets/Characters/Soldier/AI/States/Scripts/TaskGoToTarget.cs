using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    [SerializeField] float runningSpeed = 5f;

    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private void Start()
    {
        navMeshAgent = ((SoldierRunnerBT)belongingTree).NavMeshAgent;
        decisionMaker = ((SoldierRunnerBT)belongingTree).DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDestroy()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = runningSpeed;

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
        if(state == NodeState.RUNNING)
        { ClearData("target"); }
    }
}
