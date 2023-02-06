using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class CheckTargetInAttackRange : Node
{
    private Transform transform;
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private float attackThreshold = 1.5f;

    public CheckTargetInAttackRange() 
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
            if (SoldierRunnerBT.CharacterAnimator.animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f) // Avoids soldier moving while in attack animation to return to patrol state
            {
                state = NodeState.FAILURE;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        if (((target.position - transform.position).sqrMagnitude < (attackThreshold * attackThreshold)))
        {
            SoldierRunnerBT.IsTargetInAttackRange = true;
            transform.LookAt(target.position);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            SoldierRunnerBT.IsTargetInAttackRange = false;
        }
        
        state = NodeState.FAILURE;
        return state;
    }

    private void EraseInterestingTarget()
    {
        if (!SoldierRunnerBT.IsTargetInAttackRange)
        { ClearData("target"); }
    }
}
