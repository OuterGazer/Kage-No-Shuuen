using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement), typeof(CharacterStateHandler))]
public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] float cameraThresholdNearWall = 0.85f;

    private Animator animator;
    private CharacterMovement characterMovement;
    private CharacterStateHandler characterStateHandler;

    int movementForwardHash;
    int movementSidewaysHash;
    int onWallHash;

    Vector3 oldPosition;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        characterStateHandler = GetComponent<CharacterStateHandler>();
    }

    private void Start()
    {
        movementForwardHash = Animator.StringToHash("movementForward");
        movementSidewaysHash = Animator.StringToHash("movementSideways");
        onWallHash = Animator.StringToHash("OnWall");

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
        CalculateMovementDirectionProAxis();
        CalculateMovementSpeedProAxis();

        ApplyAnimationTransitionValues();
    }

    private void CalculateDistanceMovedLastFrameProAxis()
    {
        Vector3 distanceMoved = transform.position - oldPosition;
        amountOfForwardMovement = Vector3.Project(distanceMoved, transform.forward).magnitude;
        amountOfSidewaysMovement = Vector3.Project(distanceMoved, transform.right).magnitude;
    }

    private void CalculateMovementDirectionProAxis()
    {
        forwardMovementDirection = Mathf.Sign(characterMovement.MovementDirection.z);
        sidewaysMovementDirection = Mathf.Sign(characterMovement.MovementDirection.x);
    }

    private void CalculateMovementSpeedProAxis()
    {
        // Blend tree uses normalized values 1 for running speed and 0,5 for crouching speed
        // A change here must take in account a change in the blend tree
        // TODO: look for a way to decouple this
        currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / characterMovement.RunningSpeed;
        currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / characterMovement.RunningSpeed;
    }

    private void ApplyAnimationTransitionValues()
    {
        animator.SetFloat(movementForwardHash, currentVelocityForwardNormalized * forwardMovementDirection);

        // TODO: Dot Product to have the player face the right way when OnWall works but it's not robust enough. Look for an alternative.
        if (IsPlayerOnWallAndUsingWSInsteadOfAD())
        {
            if (IsPlayerForwardPointingTheSameDirectionAsCameraRight())
                animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * forwardMovementDirection);
            else
                animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * -forwardMovementDirection);
        }
        else
            { animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * sidewaysMovementDirection); }
    }

    private bool IsPlayerForwardPointingTheSameDirectionAsCameraRight()
    {
        Vector3 projectedCameraRightToUpPlane = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);
        return Vector3.Dot(projectedCameraRightToUpPlane, transform.forward) >= cameraThresholdNearWall;
    }

    private bool IsPlayerOnWallAndUsingWSInsteadOfAD()
    {
        return characterStateHandler.PlayerState.HasFlag(CharacterState.OnWall) && IsCameraNearWall();
    }

    private bool IsCameraNearWall()
    {
        float dotProductCameraForwardAndPlayerXAxis = Vector3.Dot(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up), transform.right);

        if (dotProductCameraForwardAndPlayerXAxis >= cameraThresholdNearWall ||
            dotProductCameraForwardAndPlayerXAxis <= -cameraThresholdNearWall)
            { return true; }
        else
            { return false; }
    }

    private void HaveCharacterInteractWithWall(bool isCover)
    {
        animator.SetBool(onWallHash, isCover);
    }
}
