using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterOnHookState : CharacterStateBase
{
    [SerializeField] float hookReachThreshold = 4f;
    private Transform hookTarget;
    private HookTargetChecker hookTargetChecker;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;
    

    [HideInInspector] public UnityEvent throwHook;
    [HideInInspector] public UnityEvent<bool> ChangeToHangingAnimation;
    [HideInInspector] public UnityEvent CanNotFindHookTarget;

    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;

    private Vector3 hangingDirection;

    private float currentOnHookSpeed;

    private bool isHookThrown = false;

    private void Awake()
    {
        hookTargetChecker = GetComponent<HookTargetChecker>();
        characterAnimator = GetComponent<CharacterAnimator>();
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        spineToFingerRig.weight = 0f;
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
        CheckIfThereIsAvailableHookTarget();        
    }

    private void CheckIfThereIsAvailableHookTarget()
    {
        if (hookTargetChecker.CanPerformHookThrow) 
        { 
            hookTarget = hookTargetChecker.HookTarget;
            PerformHookThrowing(); 
        }
        else 
        { ExitToIdle(); }
    }

    private void PerformHookThrowing()
    {
        SetTargetToRigChain();
        ThrowHook();
    }

    private void ExitToIdle()
    {
        CanNotFindHookTarget.Invoke(); //Event for CharacterEngine to transition to Idle if there isn't any hook target around or it's blocked
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
        return (hookTarget.position - transform.position).sqrMagnitude <= (hookReachThreshold * hookReachThreshold);
    }

    private void ExitState()
    {
        ChangeToHangingAnimation.Invoke(false);
        spineToFingerRig.weight = 0f;
        currentOnHookSpeed = -0.1f; // Needed to trigger state change to OnAir in CharacterEngine upon reaching target
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
