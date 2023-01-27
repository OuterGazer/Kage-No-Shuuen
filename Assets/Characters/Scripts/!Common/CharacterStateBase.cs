using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterStateBase : MonoBehaviour
{
    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;

    private static Camera mainCamera;
    protected static CharacterController charController;

    protected static Vector3 movementDirection;
    public static Vector3 MovementDirection => movementDirection;

    private static float velocityY = 0f;

    [HideInInspector] public UnityEvent<Vector3> onMovementSpeedChange;
    [HideInInspector] public UnityEvent onNeedingToTransitionToIdle;
    [HideInInspector] public UnityEvent onBeingOnAir;
    [HideInInspector] public UnityEvent<CharacterStateBase> onCombatStateEnteringOrExiting;

    public virtual void ExitState() { }

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        charController = characterController;
    }

    protected static float movingSpeed;
    protected static float combatStateSpeedModifier = 0f;
    protected static Vector3 currentHorizontalMovement = Vector3.zero;
    protected static float accMovementDir = 1.5f; // m/s2
    protected void UpdateMovement(float speed, Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        UpdateCharacterSpeed(speed + combatStateSpeedModifier);
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

    protected static float moveAcceleration = 5;    // m/s2
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

    // TODO: put it in its own class?
    private static float timeToOrientateCharacterForward = 0.25f;
    protected void OrientateCharacterForward()
    {
        Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

        DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

        // Alternativa sin DoTween pero con un fallo
        // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
        //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
        //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
        //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
    }

    private void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();

        // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
        if (inputBuffer != Vector3.zero)
        {
            if (inputBuffer.y != 0f)
                inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

            movementDirection = inputBuffer;

            onMovementSpeedChange.Invoke(movementDirection);
        }
        else
        {
            movementDirection = Vector3.zero;
        }
    }
}
