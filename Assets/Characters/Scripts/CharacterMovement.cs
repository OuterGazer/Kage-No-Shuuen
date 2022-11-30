using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    // Due to how animation in the blend tree works, walking speed must be always half of running speed.
    // TODO: look on a way to decouple the previous dependency.
    [SerializeField] float runningSpeed = 10f;
    [SerializeField] float crouchingSpeed = 3f;
    [SerializeField] float timeToReachFullSpeed = 0.05f;
    public float RunningSpeed => runningSpeed;
    public float CrouchingSpeed => crouchingSpeed;
    public Vector3 MovementDirection { get; private set; }

    private float velocityY;

    private CharacterController characterController;
    private Camera mainCamera;

    private CharacterState playerState;

    public Vector3 SetMovementDirection(Vector3 movementDirection)
    {
        if(movementDirection != Vector3.zero)
            return MovementDirection = movementDirection;
        else
            // When the player stops pressing a movement button, the vector becomes Vector3.zero.
            // This line returns the last value and is used to correctly deccelerate the character
            // and thus create an appropriate animation transition.
            return MovementDirection;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        playerState = CharacterState.StandIdle;
    }

    float movingSpeed;
    void Update()
    {
        UpdateCharacterState();

        Vector3 horizontalMovement = UpdateHorizontalMovement() * movingSpeed * Time.deltaTime;
        Vector3 verticalMovement = UpdateVerticalMovement();

        characterController.Move(horizontalMovement + verticalMovement);
    }

    private void UpdateCharacterState()
    {
        switch (playerState)
        {
            case CharacterState.StandIdle:
            case CharacterState.CrouchIdle:
                UpdateCharacterSpeed(0.0f);
                break;
            case CharacterState.CrouchMove:
                UpdateCharacterSpeed(crouchingSpeed);
                break;
            case CharacterState.Running:
                UpdateCharacterSpeed(runningSpeed);
                break;
        }
    }

    private void UpdateCharacterSpeed(float targetSpeed)
    {
        DOTween.To(() => this.movingSpeed, x => this.movingSpeed = x, targetSpeed, timeToReachFullSpeed);
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

        MakeCharacterAlwaysFaceForwardOnMovement(movement);

        return movement;       
    }

    private void MakeCharacterAlwaysFaceForwardOnMovement(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);            
            transform.rotation = Quaternion.LookRotation(projectedForwardVector, Vector3.up);
        }
    }

    private Vector3 ApplyMovementRelativeToCameraPosition()
    {
        Vector3 movement = mainCamera.transform.TransformDirection(MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }

    // TODO: Look for an alternative to this switch mess, there must be a simpler way.
    public void OnCrouch()
    {
        switch (playerState)
        {
            case CharacterState.Running:
                playerState = CharacterState.CrouchMove;
                break;
            case CharacterState.StandIdle:
                playerState = CharacterState.CrouchIdle;
                break;
            case CharacterState.CrouchMove:
                playerState = CharacterState.Running;
                break;
            case CharacterState.CrouchIdle:
                playerState = CharacterState.StandIdle;
                break;
        }
    }

    public void OnMove(InputValue inputValue)
    {
        if(inputValue.Get<Vector2>() != Vector2.zero)
        {
            switch (playerState)
            {
                case CharacterState.CrouchIdle:
                case CharacterState.CrouchMove:
                    playerState = CharacterState.CrouchMove;
                    break;
                case CharacterState.StandIdle:
                case CharacterState.Running:
                    playerState = CharacterState.Running;
                    break;
            }
        }
        else
        {
            switch (playerState)
            {
                case CharacterState.Running:
                    playerState = CharacterState.StandIdle;
                    break;
                case CharacterState.CrouchMove:
                    playerState = CharacterState.CrouchIdle;
                    break;
            }
        }
    }
}
