using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterInput), typeof(CharacterController))]
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

    private CharacterController characterController;
    private CharacterInput characterInput;
    private Camera mainCamera;

    private CharacterState playerState;

    private float velocityY;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterInput = GetComponent<CharacterInput>();

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
        //DOTween.To(() => transform.position, x => transform.position = x, new Vector3(2, 2, 2), 1); 
        switch (playerState)
        {
            case CharacterState.Standing:
                UpdateCharacterSpeed(movingSpeed, 0f);
                break;
            case CharacterState.Walking:
                UpdateCharacterSpeed(movingSpeed, walkingSpeed);
                break;
            case CharacterState.Crouching:
                UpdateCharacterSpeed(movingSpeed, crouchingSpeed);
                break;
            case CharacterState.Running:
                UpdateCharacterSpeed(movingSpeed, runningSpeed);
                break;
        }
    }

    private void UpdateCharacterSpeed(float movingSpeed, float targetSpeed)
    {
        this.movingSpeed = Mathf.MoveTowards(movingSpeed, targetSpeed, speedAcceleration);        
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
        Vector3 movement = mainCamera.transform.TransformDirection(characterInput.MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }


    // TODO: Look for a better way to handle state change without multiple exit states in the methods
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

    public void OnMove()
    {
        if (playerState == CharacterState.Walking)
            { return; }

        playerState = CharacterState.Running;
    }
}
