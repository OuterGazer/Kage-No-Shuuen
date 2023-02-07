using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class CheckTargetInAttackRange : Node
{
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private float attackThreshold = 1.5f;


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

    //public CheckTargetInAttackRange() 
    //{
    //    navMeshAgent = SoldierRunnerBT.NavMeshAgent;
    //    decisionMaker = SoldierRunnerBT.DecisionMaker;

    //    decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    //}

    public override NodeState Evaluate()
    {
        Transform target = (Transform) GetData("target");
        
        if (target == null)
        {
            return CheckIfAttackAnimationHasFinished();
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

        return CheckIfAttackAnimationHasFinished();
    }

    private NodeState CheckIfAttackAnimationHasFinished()
    {
        if (SoldierRunnerBT.CharacterAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f) // Avoids soldier jummping to TasGoToTarget in attack animation
        {
            state = NodeState.FAILURE;
            return state;
        }
        else
        {
            state = NodeState.RUNNING;
            return state;
        }
    }

    private void EraseInterestingTarget()
    {
        if (!SoldierRunnerBT.IsTargetInAttackRange)
        { ClearData("target"); }
    }
}
