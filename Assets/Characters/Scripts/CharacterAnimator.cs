using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterCrouchingState), typeof(CharacterOnWallState))]
[RequireComponent(typeof(CharacterOnHookState), typeof(CharacterOnAirState))]
public class CharacterAnimator : MonoBehaviour
{
    [HideInInspector] public UnityEvent hookHasArrivedAtTarget;

    private Animator animator;

    private CharacterCrouchingState crouchingState;
    private CharacterOnWallState onWallState;
    private CharacterOnHookState onHookState;
    private CharacterOnAirState onAirState;
    private CharacterDodgingState dodgingState;
    private Vector3 movementDirection;
    private float runningSpeed;

    int movementForwardHash;
    int movementSidewaysHash;
    int onWallHash;
    int throwHookHash;
    int isGroundedHash;
    int isHookedHash;
    int landingHash;
    int dodgingHash;

    Vector3 oldPosition;

    private bool isAnimationCorrectedWhenOnWall = false;

    private void Awake()
    {
        GetNecessaryComponents();
        AddNecessaryListeners();
    }

    private void GetNecessaryComponents()
    {
        animator = GetComponentInChildren<Animator>();

        runningSpeed = GetComponent<CharacterRunningState>().Speed;

        crouchingState = GetComponent<CharacterCrouchingState>();
        onWallState = GetComponent<CharacterOnWallState>();
        onHookState = GetComponent<CharacterOnHookState>();
        onAirState = GetComponent<CharacterOnAirState>();
        dodgingState = GetComponent<CharacterDodgingState>();
    }

    private void AddNecessaryListeners()
    {
        crouchingState.attachCharacterToWall.AddListener(AttachCharacterToWall);
        onWallState.removeCharacterFromWall.AddListener(RemoveCharacterFromWall);
        onWallState.correctCharacterAnimationWhenCameraIsNearWall.AddListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.AddListener(HaveCharacterThrowHook);
        onHookState.changeToHangingAnimation.AddListener(TransitionToOrFromHooked);
        onAirState.changeToLandingAnimation.AddListener(TriggerLandingAnimation);
        onAirState.isCharacterTouchingGround.AddListener(TransitionToOrFromAir);
        dodgingState.makeCharaterDodge.AddListener(PlayDodgeAnimation);
    }    

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void RemoveListeners()
    {
        crouchingState.attachCharacterToWall.RemoveListener(AttachCharacterToWall);
        onWallState.removeCharacterFromWall.RemoveListener(RemoveCharacterFromWall);
        onWallState.correctCharacterAnimationWhenCameraIsNearWall.RemoveListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.RemoveListener(HaveCharacterThrowHook);
        onHookState.changeToHangingAnimation.RemoveListener(TransitionToOrFromHooked);
        onAirState.changeToLandingAnimation.RemoveListener(TriggerLandingAnimation);
        onAirState.isCharacterTouchingGround.RemoveListener(TransitionToOrFromAir);
        dodgingState.makeCharaterDodge.RemoveListener(PlayDodgeAnimation);
    }

    private void Start()
    {
        GenerateHashes();

        animator.SetBool(isGroundedHash, true);
        oldPosition = transform.position;
    }

    private void GenerateHashes()
    {
        movementForwardHash = Animator.StringToHash("movementForward");
        movementSidewaysHash = Animator.StringToHash("movementSideways");
        onWallHash = Animator.StringToHash("OnWall");
        throwHookHash = Animator.StringToHash("throwHook");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isHookedHash = Animator.StringToHash("isHooked");
        landingHash = Animator.StringToHash("Landing");
        dodgingHash = Animator.StringToHash("Dodging");
    }

    private void Update()
    {
        UpdateStandingAnimationTransitions();

        oldPosition = transform.position;
    }

    float amountOfForwardMovement;
    float amountOfSidewaysMovement;
    float forwardMovementDirection;
    float sidewaysMovementDirection;
    float currentVelocityForwardNormalized;
    float currentVelocitySidewaysNormalized;
    private void UpdateStandingAnimationTransitions()
    {
        CalculateDistanceMovedLastFrameProAxis();
        CalculateSignOfMovementDirection();
        CalculateMovementSpeedProAxis();

        ApplyAnimationTransitionValues();
    }

    private void CalculateDistanceMovedLastFrameProAxis()
    {
        Vector3 distanceMoved = transform.position - oldPosition;
        amountOfForwardMovement = Vector3.Project(distanceMoved, transform.forward).magnitude;
        amountOfSidewaysMovement = Vector3.Project(distanceMoved, transform.right).magnitude;
    }

    private void CalculateSignOfMovementDirection()
    {
        if (IsPlayerPressingAMovementKey(movementDirection.z))
            forwardMovementDirection = Mathf.Sign(movementDirection.z);

        if (IsPlayerPressingAMovementKey(movementDirection.x))
            sidewaysMovementDirection = Mathf.Sign(movementDirection.x);

        // Releasing a key keeps the last sign, avoiding that the character
        // briefly looks the other way while decelerating when sign is -1.
    }

    private bool IsPlayerPressingAMovementKey(float movementAxis)
    {
        return !Mathf.Approximately(movementAxis, 0f);
    }

    private void CalculateMovementSpeedProAxis()
    {
        // Blend tree uses normalized values 1 for running speed and 0,5 for crouching speed
        // A change here must take in account a change in the blend tree
        // TODO: look for a way to decouple this
        currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / runningSpeed; 
        currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / runningSpeed;
    }

    private void ApplyAnimationTransitionValues()
    {
        animator.SetFloat(movementForwardHash, currentVelocityForwardNormalized * forwardMovementDirection);

        if (isAnimationCorrectedWhenOnWall)
        {
            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * (forwardMovementDirection * onWallAnimationCorrectionFactor));
            isAnimationCorrectedWhenOnWall = false;
        }
        else
        {
            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * sidewaysMovementDirection);
        }
    }

    private float onWallAnimationCorrectionFactor = 1f;
    private void SetCorrectAnimationWhenCharacterIsOnWall(float directionSign)
    {
        onWallAnimationCorrectionFactor = directionSign;
        isAnimationCorrectedWhenOnWall = true;
    }
    
    void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();

        // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
        if (inputBuffer != Vector3.zero)
        {
            if (inputBuffer.y != 0f)
                inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

            movementDirection = inputBuffer;
        }
    }

    public void AttachCharacterToWall()
    {
        animator.SetBool(onWallHash, true);
    }

    public void RemoveCharacterFromWall()
    {
        animator.SetBool(onWallHash, false);
    }

    public void HaveCharacterThrowHook()
    {
        animator.SetTrigger(throwHookHash);
    }

    public void TransitionToOrFromHooked(bool isHooked)
    {
        animator.SetBool(isHookedHash, isHooked);
    }

    public void TriggerLandingAnimation()
    {
        animator.SetTrigger(landingHash);
    }

    public void TransitionToOrFromAir(bool isGrounded)
    {
        animator.SetBool(isGroundedHash, isGrounded);
    }

    public void HookHasArrivedAtTarget()
    {
        hookHasArrivedAtTarget.Invoke();
    }

    public void PlayDodgeAnimation()
    {
        animator.SetTrigger(dodgingHash);
    }
}
