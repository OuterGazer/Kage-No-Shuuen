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
    [SerializeField] float onHookSpeed = 10f;
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

        Vector3 horizontalMovement = movingSpeed * Time.deltaTime * currentHorizontalMovement;
        Vector3 verticalMovement = UpdateVerticalMovement();

        if (characterStateHandler.PlayerState != CharacterState.OnHook &&
            characterStateHandler.PlayerState != CharacterState.OnAir)
        {
            characterController.Move(horizontalMovement + verticalMovement);
        }
        else if (characterStateHandler.PlayerState == CharacterState.OnHook)
        {
            characterController.Move(onHookSpeed * Time.deltaTime * hangingDirection);

            if ((hookTarget.position - transform.position).sqrMagnitude <= 4f)
            {
                characterStateHandler.SetCharacterOnAir();
                BroadcastMessage("TransitionToOrFromHooked", false);
            }                
        }
        else if(characterStateHandler.PlayerState == CharacterState.OnAir)
        {
            characterController.Move(verticalMovement);

            if (characterController.isGrounded)
            {
                characterStateHandler.SetCharacterOnIdle();
                BroadcastMessage("TransitionToOrFromAir", true);
            }
        }

        //Debug.Log(characterStateHandler.PlayerState);
    }

    private void FixedUpdate()
    {
        if (IsCharacterOnWall())
        {
            if (HasCoverEdgeBeenReached())
                movingSpeed = 0f;
        }
        else if (isCharacterAtCoverEdge)
            { isCharacterAtCoverEdge = false; }
    }

    private bool IsCharacterOnWall()
    {
        return characterStateHandler.PlayerState.HasFlag(CharacterState.OnWall);
    }


    [SerializeField] float stoppingDistanceToCoverEdge = 3.75f;
    private bool HasCoverEdgeBeenReached()
    {
        float characterMovementDirectionOnWall = Mathf.Sign(Vector3.Dot(transform.right, currentHorizontalMovement));

        Vector3 raycastOriginPoint = transform.position + characterController.bounds.extents.x * characterMovementDirectionOnWall *
                                                          stoppingDistanceToCoverEdge * transform.right;

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
            case CharacterState.OnHook: 
                desiredMovingSpeed = onHookSpeed;
                break;
        }

        if(!isCharacterAtCoverEdge)
            UpdateCharacterSpeed(desiredMovingSpeed);
    }

    [SerializeField] float moveAcceleration = 5;    // m/s2
    //[SerializeField] float onHookAcceleration = 10;    // m/s2
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
        //else
        //{
        //    characterStateHandler.SetCharacterOnAir();
        //    BroadcastMessage("TransitionToOrFromAir", false);
        //}

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

    private Vector3 ApplyMovementRelativeToCameraPosition(Vector3 planeToProjectMovementOn)
    {
        Vector3 movement = mainCamera.transform.TransformDirection(MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, planeToProjectMovementOn);
        return movement;
    }

    [SerializeField] Transform hookTarget;
    public void OnHookThrow()
    {
        // TODO: encontrar una manera programática de agregar un target (con un Overlap/CheckSphere?)
        if (characterStateHandler.PlayerState.HasFlag(CharacterState.Idle))
            BroadcastMessage("HaveCharacterThrowHook");
    }

    private Vector3 hangingDirection;
    public Vector3 HangingDirection => hangingDirection;
    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        characterStateHandler.SetCharacterOnHook();
        BroadcastMessage("TransitionToOrFromHooked", true);
        BroadcastMessage("TransitionToOrFromAir", false);
    }
}
