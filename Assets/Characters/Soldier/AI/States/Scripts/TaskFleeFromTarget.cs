using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTree;
using UnityEngine.Events;

public class TaskFleeFromTarget : Node
{
    [SerializeField] float fleeingSpeed = 3f;
    [SerializeField] float maxFleeDistance = 7f;

    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    public UnityEvent OnTargetLost;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        decisionMaker = ((SoldierBehaviour)belongingTree).DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDestroy()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        object isInteractionAnimationPlaying = GetData("interactionAnimation");

        navMeshAgent.stoppingDistance = 0.5f;

        if (IsTargetTooClose(target))
        {
            navMeshAgent.speed = fleeingSpeed;
            navMeshAgent.destination = transform.position - transform.forward;
            transform.LookAt(target.position);

            state = NodeState.RUNNING; 
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }

    private bool IsTargetTooClose(Transform target)
    {
        return (target.position - transform.position).sqrMagnitude < (maxFleeDistance * maxFleeDistance);
    }

    private void EraseInterestingTarget()
    {
        object t = GetData("target");
        if (t == null) { return; }

        if (state == NodeState.SUCCESS)
        {
            ClearData("target");
            OnTargetLost.Invoke();
        }
    }
}


