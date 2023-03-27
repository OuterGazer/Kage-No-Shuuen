using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CheckForHookTarget : Node
{
    [SerializeField] float hookThrowRadius = 10f;
    private Transform hookTarget;

    private NavMeshAgent navMeshAgent;
    private LayerMask hookTargetMask;
    

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        hookTargetMask = LayerMask.GetMask("HookTarget");
    }

    public override NodeState Evaluate()
    {
        object b = GetData("isMovingToHookTarget");
        if (b != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        if (navMeshAgent.destination.y <= transform.position.y)
        {
            state = NodeState.FAILURE; 
            return state;
        }

        Collider[] hookTargets = Physics.OverlapSphere(transform.position, hookThrowRadius, hookTargetMask);

        if (hookTargets.Length == 0)
        {
            state = NodeState.FAILURE;
            return state;
        }
        else
        {
            hookTarget = hookTargets[0].transform;
            Parent.Parent.SetData("hookTarget", hookTarget);

            navMeshAgent.enabled = false;

            state = NodeState.SUCCESS;
            return state;
        }
    }
}
