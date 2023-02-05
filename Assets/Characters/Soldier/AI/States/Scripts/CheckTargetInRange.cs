using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class CheckTargetInRange : Node
{
    private Transform transform;
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private float attackThreshold = 1.5f;

    public CheckTargetInRange() 
    {
        navMeshAgent = SoldierRunnerBT.NavMeshAgent;
        decisionMaker = SoldierRunnerBT.DecisionMaker;

        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
        transform = navMeshAgent.transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform) GetData("target"); 
        
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (((target.position - transform.position).sqrMagnitude < (attackThreshold * attackThreshold)))
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

    private void EraseInterestingTarget()
    {
        ClearData("target");
    }
}
