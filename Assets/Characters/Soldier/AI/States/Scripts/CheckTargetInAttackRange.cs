using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class CheckTargetInAttackRange : Node
{
    [SerializeField] float attackThreshold = 1.5f;

    private DecisionMaker decisionMaker;
    private CharacterAnimator characterAnimator;

    private bool isTargetInAttackRange = false;


    private void Start()
    {
        decisionMaker = ((SoldierRunnerBT)belongingTree).DecisionMaker;
        characterAnimator = ((SoldierRunnerBT)belongingTree).CharacterAnimator;

        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDisable()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform) GetData("target");
        
        if (target == null)
        {
            return CheckIfAttackAnimationHasFinished();
        }

        if (((target.position - transform.position).sqrMagnitude < (attackThreshold * attackThreshold)))
        {
            isTargetInAttackRange = true;
            transform.LookAt(target.position);
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            isTargetInAttackRange = false;
        }

        return CheckIfAttackAnimationHasFinished();
    }

    private NodeState CheckIfAttackAnimationHasFinished()
    {
        if (characterAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f) // Avoids soldier jummping to TasGoToTarget in attack animation
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
        if (!isTargetInAttackRange)
        { ClearData("target"); }
    }
}
