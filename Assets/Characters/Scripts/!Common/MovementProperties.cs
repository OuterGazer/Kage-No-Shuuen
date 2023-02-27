using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementProperties
{ 
    private Camera mainCamera;
    public Camera MainCamera 
    { 
        get { return mainCamera; }
        set { if (value.GetType() == typeof(Camera)) { mainCamera = value; } }
    }
    private CharacterController charController;
    public CharacterController CharController
    {
        get { return charController; }
        set { if (value.GetType() == typeof(CharacterController)) { charController = value; } }
    }

    private static float velocityY = 0f;

    private float movingSpeed;
    private Vector3 currentHorizontalMovement = Vector3.zero;
    private float accMovementDir = 1.5f; // m/s2
    public void UpdateMovement(float speed, Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        UpdateCharacterSpeed(speed);
        ApplyAccelerationSmoothingToMovingDirection(movementDirection, movementProjectionPlane);

        Vector3 horizontalMovement = movingSpeed * Time.deltaTime * currentHorizontalMovement;
        Vector3 verticalMovement = UpdateVerticalMovement();
        
        charController.Move(horizontalMovement + verticalMovement);
    }

    private void ApplyAccelerationSmoothingToMovingDirection(Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        Vector3 desiredHorizontalMovement = UpdateHorizontalMovement(movementDirection, movementProjectionPlane);
        Vector3 direction = desiredHorizontalMovement - currentHorizontalMovement;
        float speedChangeToApply = accMovementDir * Time.deltaTime;
        speedChangeToApply = Mathf.Min(speedChangeToApply, direction.magnitude);
        currentHorizontalMovement += direction.normalized * speedChangeToApply;
    }

    private float moveAcceleration = 5f;    // m/s2
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

        if (charController.isGrounded)
        {
            velocityY = -0.1f;
        }

        return new Vector3(0, velocityY, 0);
    }

    private Vector3 UpdateHorizontalMovement(Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        Vector3 movement;
        movement = ApplyMovementRelativeToCameraPosition(movementDirection, movementProjectionPlane);

        return movement;
    }

    private Vector3 ApplyMovementRelativeToCameraPosition(Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        Vector3 movement = mainCamera.transform.TransformDirection(movementDirection);
        movement = Vector3.ProjectOnPlane(movement, movementProjectionPlane);
        return movement;
    }
}
