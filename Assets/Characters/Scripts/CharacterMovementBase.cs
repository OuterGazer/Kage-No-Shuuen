using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CharacterMovementBase : MonoBehaviour
{
    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;

    private Camera mainCamera;
    private CharacterController characterController;

    protected static Vector3 movementDirection;
    public Vector3 MovementDirection => movementDirection;

    private float velocityY = 0f;

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        this.characterController = characterController;
    }

    static float movingSpeed;
    Vector3 currentHorizontalMovement = Vector3.zero;
    [SerializeField] float accMovementDir = 3f; // m/s2
    protected void UpdateMovement(float speed, Vector3 movementDirection)
    {
        UpdateCharacterSpeed(speed);
        ApplyAccelerationSmoothingToMovingDirection(movementDirection);

        Vector3 horizontalMovement = movingSpeed * Time.deltaTime * currentHorizontalMovement;
        Vector3 verticalMovement = UpdateVerticalMovement();
        
        characterController.Move(horizontalMovement + verticalMovement);

        OrientateCharacterForwardWhenMoving();

        // Tries to avoid that movementdirection has a small value that needs first to go down to zero if you stop and then wnat to move in the opposite direction you were moving
        //if (Mathf.Approximately(movingSpeed, 0f))
        //    movementDirection = Vector3.zero;
    }

    private void ApplyAccelerationSmoothingToMovingDirection(Vector3 movementDirection)
    {
        Vector3 desiredHorizontalMovement = UpdateHorizontalMovement(movementDirection);
        Vector3 direction = desiredHorizontalMovement - currentHorizontalMovement;
        float speedChangeToApply = accMovementDir * Time.deltaTime;
        speedChangeToApply = Mathf.Min(speedChangeToApply, direction.magnitude);
        currentHorizontalMovement += direction.normalized * speedChangeToApply;
    }

    [SerializeField] float moveAcceleration = 5;    // m/s2
    private void UpdateCharacterSpeed(float targetSpeed)
    {
        if (movingSpeed < targetSpeed)
        {
            movingSpeed += moveAcceleration * Time.deltaTime;
            if (movingSpeed > targetSpeed) { movingSpeed = targetSpeed; }
        }
        else if (movingSpeed > targetSpeed)
        {
            movingSpeed -= moveAcceleration * Time.deltaTime;
            if (movingSpeed < targetSpeed) { movingSpeed = targetSpeed; }
        }
    }

    private Vector3 UpdateVerticalMovement()
    {
        velocityY = Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            velocityY = -0.1f;
        }

        return new Vector3(0, velocityY, 0);
    }

    private Vector3 UpdateHorizontalMovement(Vector3 movementDirection)
    {
        Vector3 movement;
        movement = ApplyMovementRelativeToCameraPosition(movementDirection);

        return movement;
    }

    private Vector3 ApplyMovementRelativeToCameraPosition(Vector3 movementDirection)
    {
        Vector3 movement = mainCamera.transform.TransformDirection(movementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }

    // New Implementation
    private float timeToOrientateCharacterForward = 0.25f;
    private void OrientateCharacterForwardWhenMoving()
    {
        if (Mathf.Abs(movingSpeed) > 0.1f)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

            // Alternativa sin DoTween pero con un fallo
            // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
            //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
            //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
        }
    }
}
