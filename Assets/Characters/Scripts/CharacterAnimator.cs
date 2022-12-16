using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterMovement), typeof(CharacterStateHandler))]
public class CharacterAnimator : MonoBehaviour
{
    //[SerializeField] float cameraThresholdNearWall = 0.85f;

    private Animator animator;
    //old implementation
    private CharacterMovement characterMovement;
    private CharacterStateHandler characterStateHandler;

    //new implementation
    private CharacterCrouchingState crouchingState;
    private CharacterOnWallState onWallState;
    private Vector3 movementDirection;
    private float runningSpeed;

    int movementForwardHash;
    int movementSidewaysHash;
    int onWallHash;
    int throwHookHash;
    int isGroundedHash;
    int isHookedHash;

    Vector3 oldPosition;

    private bool isAnimationCorrectedWhenOnWall = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        characterStateHandler = GetComponent<CharacterStateHandler>();

        runningSpeed = GetComponent<CharacterRunningState>().Speed;

        crouchingState = GetComponent<CharacterCrouchingState>();
        onWallState = GetComponent<CharacterOnWallState>();
        crouchingState.attachCharacterToWall.AddListener(AttachCharacterToWall);
        onWallState.removeCharacterFromWall.AddListener(RemoveCharacterFromWall);
        onWallState.correctCharacterAnimationWhenCameraIsNearWall.AddListener(SetCorrectAnimationWhenCharacterIsOnWall);
    }

    private void OnDestroy()
    {
        crouchingState.attachCharacterToWall.RemoveListener(AttachCharacterToWall);
        onWallState.removeCharacterFromWall.RemoveListener(RemoveCharacterFromWall);
    }

    private void Start()
    {
        movementForwardHash = Animator.StringToHash("movementForward");
        movementSidewaysHash = Animator.StringToHash("movementSideways");
        onWallHash = Animator.StringToHash("OnWall");
        throwHookHash = Animator.StringToHash("throwHook");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isHookedHash = Animator.StringToHash("isHooked");

        animator.SetBool(isGroundedHash, true);
        oldPosition = transform.position;
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

    /// <summary>
    /// Old Implementation, erase when refactored and working well
    /// </summary>
    //private void CalculateSignOfMovementDirection()
    //{
    //    if (IsPlayerPressingAMovementKey(characterMovement.MovementDirection.z))
    //        forwardMovementDirection = Mathf.Sign(characterMovement.MovementDirection.z);

    //    if (IsPlayerPressingAMovementKey(characterMovement.MovementDirection.x))
    //        sidewaysMovementDirection = Mathf.Sign(characterMovement.MovementDirection.x);

    //    // Releasing a key keeps the last sign, avoiding that the character
    //    // briefly looks the other way while decelerating when sign is -1.
    //}

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

    /// <summary>
    /// Old Implementation, erase when new one is working correctly
    /// </summary>
    //private void CalculateMovementSpeedProAxis()
    //{
    //    // Blend tree uses normalized values 1 for running speed and 0,5 for crouching speed
    //    // A change here must take in account a change in the blend tree
    //    // TODO: look for a way to decouple this
    //    currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / characterMovement.RunningSpeed;
    //    currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / characterMovement.RunningSpeed;
    //}

    private void CalculateMovementSpeedProAxis()
    {
        // Blend tree uses normalized values 1 for running speed and 0,5 for crouching speed
        // A change here must take in account a change in the blend tree
        // TODO: look for a way to decouple this
        currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / runningSpeed; 
        currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / runningSpeed;
    }


    /// <summary>
    /// Old Implementation, erase when new one is working correctly
    /// </summary>
    //private void ApplyAnimationTransitionValues()
    //{
    //    animator.SetFloat(movementForwardHash, currentVelocityForwardNormalized * forwardMovementDirection);

    //    // TODO: Dot Product to have the player face the right way when OnWall works but it's not robust enough. Look for an alternative.
    //    if (IsPlayerOnWallAndUsingWSInsteadOfAD())
    //    {
    //        if (IsPlayerForwardPointingTheSameDirectionAsCameraRight())
    //            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * forwardMovementDirection);
    //        else
    //            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * -forwardMovementDirection);
    //    }
    //    else
    //        { animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * sidewaysMovementDirection); }
    //}

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

    /// <summary>
    /// old implementation, erase when new one works
    /// </summary>
    /// <returns></returns>
    //private bool IsPlayerOnWallAndUsingWSInsteadOfAD()
    //{
    //    return characterStateHandler.PlayerState.HasFlag(CharacterState.OnWall) && IsCameraNearWall();
    //}

    //private bool IsPlayerOnWallAndUsingWSInsteadOfAD()
    //{
    //    return IsCameraNearWall();
    //}

    //private bool IsCameraNearWall()
    //{
    //    float dotProductCameraForwardAndPlayerXAxis = Vector3.Dot(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up), transform.right);

    //    if (dotProductCameraForwardAndPlayerXAxis >= cameraThresholdNearWall ||
    //        dotProductCameraForwardAndPlayerXAxis <= -cameraThresholdNearWall)
    //        { return true; }
    //    else
    //        { return false; }
    //}

    //private bool IsPlayerForwardPointingTheSameDirectionAsCameraRight()
    //{
    //    Vector3 projectedCameraRightToUpPlane = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);
    //    return Vector3.Dot(projectedCameraRightToUpPlane, transform.forward) >= cameraThresholdNearWall;
    //}

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

    public void TransitionToOrFromAir(bool isGrounded)
    {
        // TODO: fix the fact that the falling idle animation points automatically to the global z
        animator.SetBool(isGroundedHash, isGrounded);
    }

    public void HookHasArrivedAtTarget()
    {
        SendMessage("MoveCharacterToHookTarget");
    }
}
