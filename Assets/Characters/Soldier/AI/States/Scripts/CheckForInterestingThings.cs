using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class CheckForInterestingThings : Node
{
    private DecisionMaker decisionMaker;
    private CharacterAnimator characterAnimator;

    private void Start()
    {
        decisionMaker = ((SoldierRunnerBT)belongingTree).DecisionMaker;
        characterAnimator = ((SoldierRunnerBT)belongingTree).CharacterAnimator;
        decisionMaker.OnPlayerSeen.AddListener(SetInterestingTarget);
    }

    private void OnDestroy()
    {
        decisionMaker.OnPlayerSeen.RemoveListener(SetInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        object s = GetData("searchTarget");
        object a = GetData("interactionAnimation");

        if (a != null && IsAttackAnimationFinished())
        {
            ClearData("interactionAnimation");
        }

        if (t == null)
        {
            if (a != null) // is an interaction animation playing after a current target has been erased?
            {
                state = NodeState.RUNNING;
                return state;
            }

            if (s != null) // are we looking for a target after loosing sight of it (and is thus erased from tree)?
            {
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

    private bool IsAttackAnimationFinished()
    {
        if (!characterAnimator.Animator.GetCurrentAnimatorStateInfo(1).IsName("1st Light Attack Combo")) { return false; }

        return characterAnimator.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1f; // Avoids soldier moving if interaction/attack animation is playing
    }

    private void SetInterestingTarget(Transform transform)
    {
        Parent.Parent.SetData("target", transform);
    }
}
