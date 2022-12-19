using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterOnAirState))]
public class CharacterOnHookState : CharacterMovementBase
{
    [SerializeField] float hookReachThreshold = 4f;
    [SerializeField] Transform hookTarget;
    public Transform HookTarget => hookTarget;


    [Header("Exit States")]
    [SerializeField] CharacterOnAirState onAirState;

    [Header("Miscellaneous Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;

    [HideInInspector] public UnityEvent throwHook;
    [HideInInspector] public UnityEvent<bool> changeToHangingAnimation;
    private CharacterAnimator characterAnimator;
    private ChainIKConstraint hookHandConstraint;

    private Vector3 hangingDirection;
    public Vector3 HangingDirection => hangingDirection;

    private float currentOnHookSpeed;

    private bool isHookThrown = false;

    private void Awake()
    {
        this.enabled = false;

        characterAnimator = GetComponent<CharacterAnimator>();
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        hookHandConstraint = GetComponentInChildren<ChainIKConstraint>();
        hookHandConstraint.weight = 0f;
    }

    private void OnEnable()
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
        if (hookHandConstraint.weight < 1f && !isHookThrown)
        {
            hookHandConstraint.weight += rigAlignmentToHookTargetAcceleration;
        }
        else
        {
            isHookThrown = true;
            hookHandConstraint.weight = 0f;
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
        hookHandConstraint.weight = 0f;
        this.enabled = false;
    }

    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        currentOnHookSpeed = speed;
        hookHandConstraint.weight = 0f;
        changeToHangingAnimation.Invoke(true);
    }
}
