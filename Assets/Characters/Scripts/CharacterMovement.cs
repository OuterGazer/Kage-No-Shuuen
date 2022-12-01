using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float runningSpeed = 6f;
    [SerializeField] float runningStrafeSpeed = 6f;
    [SerializeField] float crouchingSpeed = 3f;
    [SerializeField] float crouchingStrafeSpeed = 3f;
    [SerializeField] float timeToReachFullSpeed = 0.05f;
    public float RunningSpeed => runningSpeed;
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
    public float MovingSpeed => movingSpeed;
    void Update()
    {
        UpdateMovingSpeedFromCharacterState();

        Vector3 horizontalMovement = UpdateHorizontalMovement() * Mathf.Abs(movingSpeed) * Time.deltaTime;
        Vector3 verticalMovement = UpdateVerticalMovement();

        characterController.Move(horizontalMovement + verticalMovement);
    }

    private void UpdateMovingSpeedFromCharacterState()
    {
        switch (playerState)
        {
            case CharacterState.StandIdle:
            case CharacterState.CrouchIdle:
                UpdateCharacterSpeed(0.0f);
                break;

            case CharacterState.RunningForward:
                UpdateCharacterSpeed(runningSpeed);
                break;
            case CharacterState.RunningBackwards:
                UpdateCharacterSpeed(-runningSpeed);
                break;
            case CharacterState.RunningStrafeRight:
                UpdateCharacterSpeed(runningStrafeSpeed);
                break;
            case CharacterState.RunningStrafeLeft:
                UpdateCharacterSpeed(-runningStrafeSpeed);
                break;

            case CharacterState.CrouchForward:
                UpdateCharacterSpeed(crouchingSpeed);
                break;
            case CharacterState.CrouchBackwards:
                UpdateCharacterSpeed(-crouchingSpeed);
                break;
            case CharacterState.CrouchStrafeRight:
                UpdateCharacterSpeed(crouchingStrafeSpeed);
                break;
            case CharacterState.CrouchStrafeLeft:
                UpdateCharacterSpeed(-crouchingStrafeSpeed);
                break;

            case CharacterState.RunningForward | CharacterState.RunningStrafeRight:
                UpdateCharacterSpeed(runningSpeed);
                UpdateCharacterSpeed(runningStrafeSpeed);
                break;
            case CharacterState.RunningForward | CharacterState.RunningStrafeLeft:
                UpdateCharacterSpeed(runningSpeed);
                UpdateCharacterSpeed(-runningStrafeSpeed);
                break;
            case CharacterState.RunningBackwards | CharacterState.RunningStrafeRight:
                UpdateCharacterSpeed(-runningSpeed);
                UpdateCharacterSpeed(runningStrafeSpeed);
                break;
            case CharacterState.RunningBackwards | CharacterState.RunningStrafeLeft:
                UpdateCharacterSpeed(-runningSpeed);
                UpdateCharacterSpeed(-runningStrafeSpeed);
                break;

            case CharacterState.CrouchForward | CharacterState.CrouchStrafeRight:
                UpdateCharacterSpeed(crouchingSpeed);
                UpdateCharacterSpeed(crouchingStrafeSpeed);
                break;
            case CharacterState.CrouchForward | CharacterState.CrouchStrafeLeft:
                UpdateCharacterSpeed(crouchingSpeed);
                UpdateCharacterSpeed(-crouchingStrafeSpeed);
                break;
            case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeRight:
                UpdateCharacterSpeed(-crouchingSpeed);
                UpdateCharacterSpeed(crouchingStrafeSpeed);
                break;
            case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeLeft:
                UpdateCharacterSpeed(-crouchingSpeed);
                UpdateCharacterSpeed(-crouchingStrafeSpeed);
                break;
        }
    }

    private void UpdateCharacterSpeed(float targetSpeed)
    {
        DOTween.To(() => movingSpeed, x => movingSpeed = x, targetSpeed, timeToReachFullSpeed);
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

        MakeCharacterAlwaysFaceForwardOnMovement();

        return movement;       
    }

    private void MakeCharacterAlwaysFaceForwardOnMovement()
    {
        
    }

    private Vector3 ApplyMovementRelativeToCameraPosition()
    {
        Vector3 movement = mainCamera.transform.TransformDirection(MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }

    // TODO: Look for an alternative to this switch mess, there must be a simpler way.
    public void OnCrouch(InputValue inputValue)
    {
        if (!Mathf.Approximately(inputValue.Get<float>(), 0f)) 
        {
            if(playerState.HasFlag(CharacterState.CrouchForward) ||
               playerState.HasFlag(CharacterState.CrouchBackwards) ||
               playerState.HasFlag(CharacterState.CrouchStrafeLeft) ||
               playerState.HasFlag(CharacterState.CrouchStrafeRight))
                    { return; }

            switch (playerState)
            {
                case CharacterState.StandIdle:
                    playerState = CharacterState.CrouchIdle;
                    break;

                case CharacterState.RunningForward:
                    playerState = CharacterState.CrouchForward;
                    break;
                case CharacterState.RunningBackwards:
                    playerState = CharacterState.CrouchBackwards;
                    break;
                case CharacterState.RunningStrafeRight:
                    playerState = CharacterState.CrouchStrafeLeft;
                    break;
                case CharacterState.RunningStrafeLeft:
                    playerState = CharacterState.CrouchStrafeRight;
                    break;

                case CharacterState.RunningForward | CharacterState.RunningStrafeRight:
                    playerState = CharacterState.CrouchForward | CharacterState.CrouchStrafeRight;
                    break;
                case CharacterState.RunningForward | CharacterState.RunningStrafeLeft:
                    playerState = CharacterState.CrouchForward | CharacterState.CrouchStrafeLeft;
                    break;
                case CharacterState.RunningBackwards | CharacterState.RunningStrafeRight:
                    playerState = CharacterState.CrouchBackwards | CharacterState.CrouchStrafeRight;
                    break;
                case CharacterState.RunningBackwards | CharacterState.RunningStrafeLeft:
                    playerState = CharacterState.CrouchBackwards | CharacterState.CrouchStrafeLeft;
                    break;
            }
        }
        else
        {
            switch (playerState)
            {
                case CharacterState.CrouchIdle:
                    playerState = CharacterState.StandIdle;
                    break;

                case CharacterState.CrouchForward:
                    playerState = CharacterState.RunningForward;
                    break;
                case CharacterState.CrouchBackwards:
                    playerState = CharacterState.RunningBackwards;
                    break;
                case CharacterState.CrouchStrafeRight:
                    playerState = CharacterState.RunningStrafeRight;
                    break;
                case CharacterState.CrouchStrafeLeft:
                    playerState = CharacterState.RunningStrafeLeft;
                    break;

                case CharacterState.CrouchForward | CharacterState.CrouchStrafeRight:
                    playerState = CharacterState.RunningForward | CharacterState.RunningStrafeRight;
                    break;
                case CharacterState.CrouchForward | CharacterState.CrouchStrafeLeft:
                    playerState = CharacterState.RunningForward | CharacterState.RunningStrafeLeft;
                    break;
                case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeRight:
                    playerState = CharacterState.RunningBackwards | CharacterState.RunningStrafeRight;
                    break;
                case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeLeft:
                    playerState = CharacterState.RunningBackwards | CharacterState.RunningStrafeLeft;
                    break;
            }
        }
    }

    public void OnMove(InputValue inputValue)
    {
        // Need to take inputValue directly because MovementDirection is never zero to allow for deceleration.
        if(inputValue.Get<Vector2>() != Vector2.zero)
        {
            ChangeFromIdleToMovingState();
        }
        else
        {
            ChangeFromMovingToIdleState();
        }
    }

    private void ChangeFromMovingToIdleState()
    {
        switch (playerState)
        {
            case CharacterState.RunningForward:
            case CharacterState.RunningBackwards:
            case CharacterState.RunningStrafeRight:
            case CharacterState.RunningStrafeLeft:
            case CharacterState.RunningForward | CharacterState.RunningStrafeRight:
            case CharacterState.RunningForward | CharacterState.RunningStrafeLeft:
            case CharacterState.RunningBackwards | CharacterState.RunningStrafeRight:
            case CharacterState.RunningBackwards | CharacterState.RunningStrafeLeft:
                playerState = CharacterState.StandIdle;
                break;

            case CharacterState.CrouchForward:
            case CharacterState.CrouchBackwards:
            case CharacterState.CrouchStrafeRight:
            case CharacterState.CrouchStrafeLeft:
            case CharacterState.CrouchForward | CharacterState.CrouchStrafeRight:
            case CharacterState.CrouchForward | CharacterState.CrouchStrafeLeft:
            case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeRight:
            case CharacterState.CrouchBackwards | CharacterState.CrouchStrafeLeft:
                playerState = CharacterState.CrouchIdle;
                break;
        }
    }

    
    private void ChangeFromIdleToMovingState()
    {
        switch (MovementDirection)
        {
            case Vector3 i when i == Vector3.forward:
                SetAppropriateIdleToMovingState(CharacterState.RunningForward, CharacterState.CrouchForward);
                break;
            case Vector3 i when i == Vector3.back:
                SetAppropriateIdleToMovingState(CharacterState.RunningBackwards, CharacterState.CrouchBackwards);
                break;
            case Vector3 i when i == Vector3.right:
                SetAppropriateIdleToMovingState(CharacterState.RunningStrafeRight, CharacterState.CrouchStrafeRight);
                break;
            case Vector3 i when i == Vector3.left:
                SetAppropriateIdleToMovingState(CharacterState.RunningStrafeLeft, CharacterState.CrouchStrafeLeft);
                break;

            case Vector3 i when i == (Vector3.forward + Vector3.right):
                SetAppropriateIdleToMovingState(CharacterState.RunningForward | CharacterState.RunningStrafeRight,
                                                CharacterState.CrouchForward| CharacterState.CrouchStrafeRight);
                break;
            case Vector3 i when i == (Vector3.forward + Vector3.left):
                SetAppropriateIdleToMovingState(CharacterState.RunningForward | CharacterState.RunningStrafeLeft,
                                                CharacterState.CrouchForward | CharacterState.CrouchStrafeLeft);
                break;
            case Vector3 i when i == (Vector3.back + Vector3.right):
                SetAppropriateIdleToMovingState(CharacterState.RunningBackwards | CharacterState.RunningStrafeRight,
                                                CharacterState.CrouchBackwards | CharacterState.CrouchStrafeRight);
                break;
            case Vector3 i when i == (Vector3.back + Vector3.left):
                SetAppropriateIdleToMovingState(CharacterState.RunningBackwards | CharacterState.RunningStrafeLeft,
                                                CharacterState.CrouchBackwards | CharacterState.CrouchStrafeLeft);
                break;
        }
    }

    private void SetAppropriateIdleToMovingState(CharacterState inPossibleState1, CharacterState inPossibleState2)
    {
        switch (playerState)
        {
            case CharacterState.StandIdle:
            case CharacterState i when i == inPossibleState1:
                playerState = inPossibleState1;
                break;
            case CharacterState.CrouchIdle:
            case CharacterState i when i == inPossibleState2:
                playerState = inPossibleState2;
                break;
        }
    }
}
