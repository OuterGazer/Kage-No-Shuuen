using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using TMPro;

public class CheckTargetIsWithinDistance : Node
{
    [SerializeField] float interactionDistanceThreshold = 5f;

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (IsTargetWithinInteractionDistance(target))
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }

    private bool IsTargetWithinInteractionDistance(Transform target)
    {
        return (target.position - transform.position).sqrMagnitude < (interactionDistanceThreshold * interactionDistanceThreshold);
    }
}
