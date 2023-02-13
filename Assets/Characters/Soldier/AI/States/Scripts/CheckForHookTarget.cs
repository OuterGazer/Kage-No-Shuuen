using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CheckForHookTarget : Node
{
    [SerializeField] float hookThrowRadius = 10f;
    [SerializeField] float hookReachThreshold = 4f;
    [SerializeField] float onHookSpeed = 10f;
    private Transform hookTarget;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;
    private LayerMask hookTargetMask;

    private Vector3 hangingDirection;
    private bool isHookThrown = false;
    private bool isMovingToHookTarget = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        spineToFingerRig.weight = 0f;
        hookTargetMask = LayerMask.GetMask("HookTarget");
    }

    private void OnDestroy()
    {
        characterAnimator.hookHasArrivedAtTarget.RemoveListener(MoveCharacterToHookTarget);
    }

    public override NodeState Evaluate()
    {
        if (isMovingToHookTarget)
        {
            PointArmTowardsHookTarget();

            navMeshAgent.destination = hookTarget.position;

            if (IsHookTargetReached())
            {
                ExitState();

                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING; 
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
            PerformHookThrowing();
        }

        state = NodeState.RUNNING; 
        return state;
    }

    private void PointArmTowardsHookTarget()
    {
        if (spineToFingerRig.weight < 1f && !isHookThrown)
        {
            spineToFingerRig.weight += rigAlignmentToHookTargetAcceleration * Time.deltaTime;
        }
        else
        {
            isHookThrown = true;
            spineToFingerRig.weight = 0f;
        }
    }

    private bool IsHookTargetReached()
    {
        return (hookTarget.position - transform.position).sqrMagnitude <= hookReachThreshold;
    }

    private void ExitState()
    {
        characterAnimator.TransitionToOrFromHooked(false);
        spineToFingerRig.weight = 0f;
        onHookSpeed = -0.1f; // Needed to trigger state change to OnAir in CharacterEngine upon reaching target

        isMovingToHookTarget = false;

        hookTarget = null;
        SetTargetToRigChain();
    }

    private void PerformHookThrowing()
    {
        SetTargetToRigChain();
        ThrowHook();
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

    private void ThrowHook()
    {
        transform.forward = Vector3.ProjectOnPlane(hookTarget.position, Vector3.up);

        characterAnimator.HaveCharacterThrowHook();

        isHookThrown = false;
        hangingDirection = Vector3.zero;
    }

    // Called from animation event
    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        navMeshAgent.speed = onHookSpeed;
        spineToFingerRig.weight = 0f;
        characterAnimator.TransitionToOrFromHooked(true);
        isMovingToHookTarget = true;
    }
}
