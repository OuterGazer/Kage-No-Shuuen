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
    [SerializeField] float walkingSpeed = 3f;
    [SerializeField] float crouchingSpeed = 3f;
    [SerializeField] float speedAcceleration = 0.05f;
    public float RunningSpeed => runningSpeed;
    public float WalkingSpeed => walkingSpeed;
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
        playerState = CharacterState.Standing;
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
            case CharacterState.Standing:
                UpdateCharacterSpeed(movingSpeed, 0.0f, speedAcceleration);
                break;
            case CharacterState.Walking:
                UpdateCharacterSpeed(movingSpeed, walkingSpeed, speedAcceleration);
                break;
            case CharacterState.Crouching:
                UpdateCharacterSpeed(movingSpeed, crouchingSpeed, speedAcceleration);
                break;
            case CharacterState.Running:
                UpdateCharacterSpeed(movingSpeed, runningSpeed, speedAcceleration);
                break;
        }
    }

    private void UpdateCharacterSpeed(float movingSpeed, float targetSpeed, float speedAcceleration)
    {
        //this.movingSpeed = Mathf.MoveTowards(movingSpeed, targetSpeed, speedAcceleration);
        DOTween.To(() => this.movingSpeed, x => this.movingSpeed = x, targetSpeed, speedAcceleration);
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


    // TODO: Look for a better way to handle state change without multiple exit states in the methods
    // TODO: fix bug where if we are already walking or crouching in a given direction, changing direction makes us run.
    public void OnWalk()
    {
        if(playerState == CharacterState.Walking) 
            { playerState = CharacterState.Running; return; }
        
        playerState = CharacterState.Walking;
    }

    public void OnCrouch()
    {
        if (playerState == CharacterState.Crouching)
        { playerState = CharacterState.Running; return; }

        playerState = CharacterState.Crouching;
    }

    public void OnMove(InputValue inputValue)
    {
        if (playerState == CharacterState.Walking)
            { return; }

        if(inputValue.Get<Vector2>() != Vector2.zero)
            playerState = CharacterState.Running;
        else
            playerState = CharacterState.Standing;
    }
}
