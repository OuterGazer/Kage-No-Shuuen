using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float interactionDistanceThreshold = 1.5f;

    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;

    private void Start()
    {
        navMeshAgent = ((SoldierRunnerBT)belongingTree).NavMeshAgent;
        decisionMaker = ((SoldierRunnerBT)belongingTree).DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseInterestingTarget);
    }

    private void OnDestroy()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseInterestingTarget);
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target)
        {
            if (((target.position - transform.position).sqrMagnitude > (interactionDistanceThreshold * interactionDistanceThreshold)))
            {
                if (GetData("interactionAnimation") == null)
                {
                    navMeshAgent.destination = target.position;
                    navMeshAgent.speed = runningSpeed;
                }

                state = NodeState.RUNNING;
                return state;
            }
            else
            {
                transform.LookAt(target.position);
                state = NodeState.SUCCESS;
                return state;
            }                
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void EraseInterestingTarget()
    {
        if(state == NodeState.RUNNING)
        { ClearData("target"); }
    }
}
