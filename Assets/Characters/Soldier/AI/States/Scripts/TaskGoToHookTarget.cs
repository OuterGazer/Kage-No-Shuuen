using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using System;

public class TaskGoToHookTarget : Node
{
    [SerializeField] float onHookSpeed = 10f;
    [SerializeField] float hookReachThreshold = 4f;
    private Transform hookTarget;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 1f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;

    private Vector3 hangingDirection;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        
        spineToFingerRig = GetComponentInChildren<Rig>();
    }

    public override NodeState Evaluate()
    {
        object b = GetData("isMovingToHookTarget");
        if (b == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        PointArmTowardsHookTarget();

        hangingDirection = (Vector3)GetData("hangingDirection");

        transform.position += (onHookSpeed * Time.deltaTime) * hangingDirection;

        if (IsHookTargetReached())
        {
            ExitState();

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void PointArmTowardsHookTarget()
    {
        object b = GetData("isHookThrown");

        if (spineToFingerRig.weight < 1f && (b != null))
        {
            spineToFingerRig.weight += rigAlignmentToHookTargetAcceleration * Time.deltaTime;
        }
        else
        {
            ClearData("isHookThrown");
            spineToFingerRig.weight = 0f;
        }
    }

    private bool IsHookTargetReached()
    {
        hookTarget = (Transform)GetData("hookTarget");
        return (hookTarget.position - transform.position).sqrMagnitude <= (hookReachThreshold * hookReachThreshold);
    }

    private void ExitState()
    {
        characterAnimator.TransitionToOrFromHooked(false);
        spineToFingerRig.weight = 0f;
        characterAnimator.TriggerLandingAnimation();

        ClearData("isMovingToHookTarget");

        navMeshAgent.enabled = true;

        ClearData("hookTarget");
        SetTargetToRigChain();
    }

    private void SetTargetToRigChain()
    {
        foreach (ChainIKConstraint item in shoulderToFingerConstraints)
        {
            item.data.target = hookTarget;
        }
        AssignConstraintTargetToHead();
        rigBuilder.Build();
    }

    private void AssignConstraintTargetToHead()
    {
        WeightedTransform weightedTransform = new WeightedTransform();
        weightedTransform.transform = hookTarget;
        weightedTransform.weight = 1.0f;
        headConstraint.data.sourceObjects = new WeightedTransformArray() { weightedTransform };
    }
}
