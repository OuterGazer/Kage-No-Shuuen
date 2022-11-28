using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterInput), typeof(CharacterMovement))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterInput characterInput;
    private CharacterMovement characterMovement;

    int movementForwardHash;
    int movementSidewaysHash;
    int crouchHash;

    Vector3 oldPosition;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterInput= GetComponent<CharacterInput>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        movementForwardHash = Animator.StringToHash("movementForward");
        movementSidewaysHash = Animator.StringToHash("movementSideways");
        crouchHash = Animator.StringToHash("Crouch");

        oldPosition = transform.position;
    }

    private void Update()
    {
        // TODO: a�adir una ligera aceleracion con DoTween para que las animaciones del blend tree sean m�s fluidas y no pasen de una a otra en un solo frame.
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
        forwardMovementDirection = Mathf.Sign(characterInput.MovementDirection.z);
        sidewaysMovementDirection = Mathf.Sign(characterInput.MovementDirection.x);
    }

    private void CalculateMovementSpeedProAxis()
    {
        // Blend tree uses normalized values 1 for running speed and 0,5 for walking speed
        // A change here must take in account a change in the blend tree
        // TODO: look for a way to decouple this
        currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / characterMovement.RunningSpeed;
        currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / characterMovement.RunningSpeed;
    }

    private void ApplyAnimationTransitionValues()
    {
        animator.SetFloat(movementForwardHash, currentVelocityForwardNormalized * forwardMovementDirection);
        animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * sidewaysMovementDirection);
    }

    public void OnCrouch()
    {
        animator.SetTrigger(crouchHash);
    }
}
