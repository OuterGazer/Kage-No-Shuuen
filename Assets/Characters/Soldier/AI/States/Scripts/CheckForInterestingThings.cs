using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Events;

public class CheckForInterestingThings : Node
{
    private DecisionMaker decisionMaker;

    public UnityEvent OnTargetSeen;

    private void Start()
    {
        decisionMaker = ((SoldierBehaviour)belongingTree).DecisionMaker;
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

        if (t == null)
        {
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

    // Called from event raised in DecisionMaker
    private void SetInterestingTarget(Transform transform)
    {
        if(GetData("isDead") != null) { return; }

        Parent.Parent.SetData("target", transform);
        OnTargetSeen.Invoke();
    }
}
