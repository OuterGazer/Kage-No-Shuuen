using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterOnAirState))]
public class CharacterOnHookState : CharacterMovementBase
{
    // TODO: pensar en si quiero que el jugador pueda lanzar el gancho estando OnWall

    [SerializeField] float hookThrowRadius = 10f;
    [SerializeField] float hookReachThreshold = 4f;
    private Transform hookTarget;


    [Header("Exit States")]
    [SerializeField] CharacterOnAirState onAirState;
    [SerializeField] CharacterIdleState idleState;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    

    [HideInInspector] public UnityEvent throwHook;
    [HideInInspector] public UnityEvent<bool> changeToHangingAnimation;
    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;
    private LayerMask hookTargetMask;

    private Vector3 hangingDirection;
    public Vector3 HangingDirection => hangingDirection;

    private float currentOnHookSpeed;

    private bool isHookThrown = false;

    private void Awake()
    {
        this.enabled = false;

        characterAnimator = GetComponent<CharacterAnimator>();
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        spineToFingerRig.weight = 0f;
        hookTargetMask = LayerMask.GetMask("HookTarget");
    }

    private void OnEnable()
    {
        GetHookTargetNearby();
    }

    private void OnDisable()
    {
        hookTarget = null;
        SetTargetToRigChain();        
    }

    private void GetHookTargetNearby()
    {
        Collider[] hookTargets = Physics.OverlapSphere(transform.position, hookThrowRadius, hookTargetMask);

        if (hookTargets.Length == 0)
        {
            StartCoroutine(ExitToIdle());
        }            
        else
        {
            hookTarget = hookTargets[0].transform;
            SetTargetToRigChain();

            ThrowHook();
        }
    }

    private IEnumerator ExitToIdle()
    {
        this.enabled = false;
        yield return new WaitForEndOfFrame();
        idleState.enabled = true;
        
    }

    private void SetTargetToRigChain()
    {
        foreach (ChainIKConstraint item in shoulderToFingerConstraints)
        {
            item.data.target = hookTarget;
        }
        rigBuilder.Build();
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
            spineToFingerRig.weight += rigAlignmentToHookTargetAcceleration;

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
        changeToHangingAnimation.Invoke(false);

        onAirState.enabled = true;
        spineToFingerRig.weight = 0f;
        this.enabled = false;
    }

    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        currentOnHookSpeed = speed;
        spineToFingerRig.weight = 0f;
        changeToHangingAnimation.Invoke(true);
    }
}
