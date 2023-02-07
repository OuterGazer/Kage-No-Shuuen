using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckForInterestingThings : Node
{
    private DecisionMaker decisionMaker;
    private Transform interestingTransform;

    private void Start()
    {
        decisionMaker = SoldierRunnerBT.DecisionMaker;
        decisionMaker.OnPlayerSeen.AddListener(SetInterestingTarget);
    }

    private void OnDisable()
    {
        decisionMaker.OnPlayerSeen.RemoveListener(SetInterestingTarget);
    }

    //public CheckForInterestingThings()
    //{
    //    decisionMaker = SoldierRunnerBT.DecisionMaker;
    //    decisionMaker.OnPlayerSeen.AddListener(SetInterestingTarget);
    //}

    public override NodeState Evaluate()
    {
        object t = GetData("target");

        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

    private void SetInterestingTarget(Transform transform)
    {
        interestingTransform = transform;

        Parent.Parent.SetData("target", interestingTransform);
    }
}
