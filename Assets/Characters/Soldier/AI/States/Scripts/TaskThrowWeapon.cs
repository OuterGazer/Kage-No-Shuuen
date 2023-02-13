using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskThrowWeapon : Node
{
    [SerializeField] float distanceThresholdForThrowing = 5f;
    [SerializeField] float timeBetweenThrows = 5f;
    [SerializeField] ThrowingWeaponBase throwWeapon;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;

    private float throwCounter = 5f;
    private bool isThrowingAnimationRunning = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
    }

    public override NodeState Evaluate()
    {
        if (isThrowingAnimationRunning)
        {
            state = NodeState.RUNNING;
            return state;
        }

        object t = GetData("target");
        Transform target = (Transform)t;

        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        throwCounter += Time.deltaTime;
        if (IsTargetCloserThanThrowingThreshold(target))
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        if(throwCounter <= timeBetweenThrows)
        {
            state = NodeState.FAILURE;
            return state;
        }

        navMeshAgent.speed = 0f;
        throwWeapon.gameObject.SetActive(true);
        transform.LookAt(target);
        characterAnimator.PlayThrowingAnimation();
        isThrowingAnimationRunning = true;
        throwCounter = 0f;

        state = NodeState.RUNNING;
        return state;
    }

    private bool IsTargetCloserThanThrowingThreshold(Transform target)
    {
        return (target.position - transform.position).sqrMagnitude < (distanceThresholdForThrowing * distanceThresholdForThrowing);
    }

    // Called from an animation event
    public void ThrowWeapon()
    {
        throwWeapon?.Throw();
        throwWeapon.gameObject.SetActive(false);
    }

    // Called from an animation event
    public void ExitThrowingState()
    {
        isThrowingAnimationRunning = false;
    }
}
