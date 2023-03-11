using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System;

public class TaskGoToTarget : Node
{
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float interactionDistanceThreshold = 1.5f;
    [SerializeField] float playerFollowDelay = 1f;

    private float playerFollowDelayCounter = 1f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private DecisionMaker decisionMaker;
    private Transform currentTarget;
    private Vector3 targetPosition = Vector3.zero;


    private bool isSearching = false;

    private void Start()
    {
        player = ((SoldierBehaviour)belongingTree).Player;
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
        Transform searchTarget = (Transform)GetData("searchTarget");
        object isInteractionAnimationPlaying = GetData("interactionAnimation");

        currentTarget = target ? target : searchTarget;
        isSearching = searchTarget ? true : false;

        if (currentTarget)
        {
            targetPosition = currentTarget.position;
            navMeshAgent.stoppingDistance = interactionDistanceThreshold;

            playerFollowDelayCounter += Time.deltaTime;

            if (IsTargetWithinInteractionDistance())
            {
                if (isSearching && !target) // Have we lost sight of target and are searching it?
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                if (isInteractionAnimationPlaying != null)
                {
                    state = NodeState.SUCCESS;
                    return state;
                }
                else if(playerFollowDelayCounter >= playerFollowDelay) 
                {
                    navMeshAgent.speed = runningSpeed;
                    navMeshAgent.destination = targetPosition;
                }

                state = NodeState.RUNNING;
                return state;
            }
            else
            {
                playerFollowDelayCounter = 0f;
                transform.LookAt(targetPosition);
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }

    private bool IsTargetWithinInteractionDistance()
    {
        return (targetPosition - transform.position).sqrMagnitude > (interactionDistanceThreshold * interactionDistanceThreshold);
    }

    private void EraseInterestingTarget()
    {
        object t = GetData("target");
        if(t == null) { return; }

        if (state == NodeState.RUNNING)
        {
            if (player)
            {
                Parent.SetData("searchTarget", (Transform)t);
                isSearching = true;
            }
            
            ClearData("target");
        }
    }
}
