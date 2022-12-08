using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float crouchingSpeed = 3f;
    public float RunningSpeed => runningSpeed;
    public Vector3 MovementDirection { get; private set; }

    private float velocityY;

    private CharacterController characterController;
    private Camera mainCamera;

    private CharacterState playerState;
    public CharacterState PlayerState => playerState;

    public Vector3 SetMovementDirection(Vector3 movementDirection)
    {
        return MovementDirection = movementDirection;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        playerState = CharacterState.Idle;
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

        Debug.Log(playerState);
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

        switch (playerState)
        {
            case CharacterState i when i.HasFlag(CharacterState.Idle): // Idle can combine itself with crouching.
                desiredMovingSpeed = 0.0f; 
                break;
            case CharacterState i when i.HasFlag(CharacterState.Crouching):
                desiredMovingSpeed = crouchingSpeed;
                break;
            case CharacterState.Running:
                desiredMovingSpeed = runningSpeed;
                break;
        }

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
        Vector3 movement = ApplyMovementRelativeToCameraPosition();

        return movement;       
    }

    private Vector3 ApplyMovementRelativeToCameraPosition()
    {
        Vector3 movement = mainCamera.transform.TransformDirection(MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }

    // TODO: eventually these methods need to be moved out, as they break the SRP
    public void OnCrouch(InputValue inputValue)
    {
        if (IsCrouchButtonPressed(inputValue))
        {
            if (playerState.HasFlag(CharacterState.OnWall))
            {
                BroadcastMessage("HaveCharacterInteractWithWall", false);
                playerState = CharacterState.Idle | CharacterState.Crouching;
            }
            if (playerState == CharacterState.Idle)
                { playerState = CharacterState.Idle | CharacterState.Crouching; }
            else if (playerState == CharacterState.Running)
                { playerState = CharacterState.Crouching; }
        }
        else
        {
            if (playerState.HasFlag(CharacterState.OnWall))
                { } // Do nothing for now
            else if (playerState.HasFlag(CharacterState.Idle))
                { playerState = CharacterState.Idle; }
            else if (playerState == CharacterState.Crouching)
                { playerState = CharacterState.Running; }
        }
    }

    private static bool IsCrouchButtonPressed(InputValue inputValue)
    {
        return !Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    public void OnMove(InputValue inputValue)
    {
        if(inputValue.Get<Vector2>() != Vector2.zero)
        {
            if (playerState.HasFlag(CharacterState.Idle))
            {
                if (playerState.HasFlag(CharacterState.OnWall))
                    { playerState = CharacterState.Crouching | CharacterState.OnWall; }
                else if (playerState.HasFlag(CharacterState.Crouching))
                    { playerState = CharacterState.Crouching; }
                else
                    { playerState = CharacterState.Running; }                
            }
        }
        else
        {
            if (playerState == CharacterState.Running)
                { playerState = CharacterState.Idle; }
            else if((playerState.HasFlag(CharacterState.OnWall)))
                { playerState = CharacterState.Idle | CharacterState.Crouching | CharacterState.OnWall; }
            else if (playerState == CharacterState.Crouching)
                { playerState = CharacterState.Idle | CharacterState.Crouching; }
        }
    }

    [SerializeField] float timeToChangeFromWallToFreeMove = 0.25f;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Cover"))
        {
            if(playerState.HasFlag(CharacterState.Crouching))
            {
                playerState = CharacterState.Idle | CharacterState.Crouching | CharacterState.OnWall;
                BroadcastMessage("HaveCharacterInteractWithWall", true);

                DOTween.To(() => transform.forward, x => transform.forward = x, hit.normal, timeToChangeFromWallToFreeMove);
            }
        }     
    }
}
