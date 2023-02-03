using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskCheckForInterestingThings : Node
{
    private DecisionMaker decisionMaker;
    private Transform interestingTransform;
    public TaskCheckForInterestingThings(DecisionMaker decisionMaker)
    {
        this.decisionMaker = decisionMaker;
        this.decisionMaker.OnPlayerSeen.AddListener(SetInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");

        if (t == null)
        {
            if (interestingTransform)
            {
                Parent.Parent.SetData("target", interestingTransform);
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

    private void SetInterestingTarget(Transform transform)
    {
        interestingTransform = transform;
    }
}
