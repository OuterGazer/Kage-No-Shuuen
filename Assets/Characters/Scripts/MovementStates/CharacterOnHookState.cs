using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterOnHookState : CharacterStateBase
{
    [SerializeField] float hookThrowRadius = 10f;
    [SerializeField] float hookReachThreshold = 4f;
    private Transform hookTarget;


    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;
    

    [HideInInspector] public UnityEvent throwHook;
    [HideInInspector] public UnityEvent<bool> ChangeToHangingAnimation;
    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;
    private LayerMask hookTargetMask;
    private LayerMask obstaclesMask = ~(1 << 3);

    private Vector3 hangingDirection;

    private float currentOnHookSpeed;

    private bool isHookThrown = false;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        spineToFingerRig.weight = 0f;
        hookTargetMask = LayerMask.GetMask("HookTarget");

        this.enabled = false;
    }

    private void OnDestroy()
    {
        characterAnimator.hookHasArrivedAtTarget.RemoveListener(MoveCharacterToHookTarget);
    }

    private void OnEnable()
    {
        ManageHookThrowing();
    }

    private void OnDisable()
    {
        hookTarget = null;
        SetTargetToRigChain();
    }

    private void ManageHookThrowing()
    {
        CheckIfThereIsHookTargetInRange();        
    }

    private void CheckIfThereIsHookTargetInRange()
    {
        Collider[] hookTargets = Physics.OverlapSphere(transform.position, hookThrowRadius, hookTargetMask);

        if (hookTargets.Length == 0)
        {
            ExitToIdle();
        }
        else
        {
            hookTarget = hookTargets[0].transform;
            CheckIfObstaclesBetweenCharacterAndTarget();
        }
    }

    private void CheckIfObstaclesBetweenCharacterAndTarget()
    {
        Vector3 tempHookDir = (hookTarget.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, tempHookDir, hookThrowRadius + 1f, obstaclesMask);

        if (!hits[0].collider.CompareTag("HookTarget"))
        {
            ExitToIdle();
        }
        else
        {
            idleState.move.Disable(); // This goes here instead OnEnable() in OnAirState because if I press direction buttons while flying I transition to running and break things.
            PerformHookThrowing();
        }
    }

    private void PerformHookThrowing()
    {
        SetTargetToRigChain();
        ThrowHook();
    }

    private void ExitToIdle()
    {
        onNeedingToTransitionToIdle.Invoke(); //Event for CharacterEngine to transition to Idle if there isn't any hook target around or it's blocked
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

        throwHook.Invoke();

        isHookThrown = false;
        currentOnHookSpeed = 0f;
        hangingDirection = Vector3.zero;
    }

    private void Update()
    {
        PointArmTowardsHookTarget();

        charController.Move(currentOnHookSpeed * Time.deltaTime * hangingDirection);

        if (IsHookTargetReached())
        {
            ExitState();
        }
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

    private new void ExitState()
    {
        ChangeToHangingAnimation.Invoke(false);
        spineToFingerRig.weight = 0f;

        onBeingOnAir.Invoke();
    }

    // Called through an animation event
    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        currentOnHookSpeed = speed;
        spineToFingerRig.weight = 0f;
        ChangeToHangingAnimation.Invoke(true);
    }
}
