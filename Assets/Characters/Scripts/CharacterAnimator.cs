using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(CharacterMovement))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private CharacterMovement characterMovement;
    private CharacterController characterController;

    int movementForward;
    int movementSideways;

    Vector3 oldPosition;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        movementForward = Animator.StringToHash("movementForward");
        movementSideways = Animator.StringToHash("movementSideways");

        oldPosition = transform.position;
    }

    private void Update()
    {
        Vector3 distanceMoved = transform.position - oldPosition;

        Vector3 amountOfForwardMovement = Vector3.Project(distanceMoved, transform.forward);
        Vector3 amountOfSidewaysMovement = Vector3.Project(distanceMoved, transform.right);

        float forwardMovementDirection = Mathf.Sign(amountOfForwardMovement.z);
        float sidewaysMovementDirection = Mathf.Sign(amountOfSidewaysMovement.x);

        float currentVelocityForward = (amountOfForwardMovement.magnitude / Time.deltaTime) / characterMovement.Speed;
        float currentVelocitySideways = (amountOfSidewaysMovement.magnitude / Time.deltaTime) / characterMovement.Speed;

        animator.SetFloat(movementForward, currentVelocityForward * forwardMovementDirection);
        animator.SetFloat(movementSideways, currentVelocitySideways * sidewaysMovementDirection);

        oldPosition = transform.position;
    }
}
