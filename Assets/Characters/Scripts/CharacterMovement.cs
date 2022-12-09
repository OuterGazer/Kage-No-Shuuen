using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController), typeof(CharacterStateHandler))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float crouchingSpeed = 3f;
    public float RunningSpeed => runningSpeed;
    public Vector3 MovementDirection { get; private set; }

    private float velocityY;
    private LayerMask coverMask;

    private CharacterController characterController;
    private Camera mainCamera;
    private CharacterStateHandler characterStateHandler;

    private bool isCharacterAtCoverEdge = false;

    public Vector3 SetMovementDirection(Vector3 movementDirection)
    {
        return MovementDirection = movementDirection;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        mainCamera = Camera.main;

        characterStateHandler = GetComponent<CharacterStateHandler>();
    }

    private void Start()
    {
        coverMask = LayerMask.GetMask("Cover");
    }

    float movingSpeed;
    public float MovingSpeed => movingSpeed;
    Vector3 currentHorizontalMovement = Vector3.zero;
    [SerializeField] float accMovementDir = 3f; // m/s2
    void Update()
    {
        UpdateMovingSpeedFromCharacterState();
        ApplyAccelerationSmoothingToMovingDirection();

        Vector3 horizontalMovement = currentHorizontalMovement * movingSpeed * Time.deltaTime;
        Vector3 verticalMovement = UpdateVerticalMovement();

        characterController.Move(horizontalMovement + verticalMovement);
    }

    private void FixedUpdate()
    {
        if (IsCharacterOnWall())
        {
            if (HasCoverEdgeBeenReached(Mathf.Sign(currentHorizontalMovement.x)))
                movingSpeed = 0f;
        }
        else if (isCharacterAtCoverEdge)
            { isCharacterAtCoverEdge = false; }
    }
    [SerializeField] float stoppingDistanceToCoverEdge = 3.75f;
    private bool HasCoverEdgeBeenReached(float movementDirectionSign)
    {
        Vector3 raycastOriginPoint;
        if (MovementDirection != Vector3.zero)
            raycastOriginPoint = transform.localPosition + (new Vector3(0f, 0.5f, (characterController.bounds.extents.x * stoppingDistanceToCoverEdge) * -movementDirectionSign));
        else
            raycastOriginPoint = transform.localPosition;

        return isCharacterAtCoverEdge = !Physics.Raycast(raycastOriginPoint, -transform.forward, 0.5f, coverMask);
    }

    private void ApplyAccelerationSmoothingToMovingDirection()
    {
        Vector3 desiredHorizontalMovement = UpdateHorizontalMovement();
        Vector3 direction = desiredHorizontalMovement - currentHorizontalMovement;
        float speedChangeToApply = accMovementDir * Time.deltaTime;
        speedChangeToApply = Mathf.Min(speedChangeToApply, direction.magnitude);
        currentHorizontalMovement += direction.normalized * speedChangeToApply;
    }

    private void UpdateMovingSpeedFromCharacterState()
    {
        float desiredMovingSpeed = 0f;

        switch (characterStateHandler.PlayerState)
        {
            case CharacterState i when i.HasFlag(CharacterState.Idle): // Idle can combine itself with crouching.
                desiredMovingSpeed = 0.0f; 
                break;
            case CharacterState i when i.HasFlag(CharacterState.Crouching): // Crouching can combine itself with OnWall
                desiredMovingSpeed = crouchingSpeed;
                break;
            case CharacterState.Running:
                desiredMovingSpeed = runningSpeed;
                break;
        }

        if(!isCharacterAtCoverEdge)
            UpdateCharacterSpeed(desiredMovingSpeed);
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
        { velocityY = 0; }

        return new Vector3(0, velocityY, 0);
    }

    private Vector3 UpdateHorizontalMovement()
    {
        Vector3 movement;
        if (IsCharacterOnWall())
        { movement = ApplyMovementRelativeToCameraPosition(transform.forward); }
        else
        { movement = ApplyMovementRelativeToCameraPosition(Vector3.up); }

        return movement;
    }

    private bool IsCharacterOnWall()
    {
        return characterStateHandler.PlayerState.HasFlag(CharacterState.OnWall);
    }

    private Vector3 ApplyMovementRelativeToCameraPosition(Vector3 planeToProjectMovementOn)
    {
        Vector3 movement = mainCamera.transform.TransformDirection(MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, planeToProjectMovementOn);
        return movement;
    }
}
