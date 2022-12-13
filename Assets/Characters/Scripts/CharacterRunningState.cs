using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
[RequireComponent(typeof(CharacterIdleState), typeof(CharacterCrouchingState))]
public class CharacterRunningState : CharacterMovementBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterIdleState idleState;
    [SerializeField] CharacterCrouchingState crouchingState;

    private void Awake()
    {
        this.enabled = false;
    }

    void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

        OrientateCharacterForwardWhenMoving();
    }

    // TODO: refactor this OnMove repeated code from CharacterRunningState
    void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            Vector3 inputBuffer = inputValue.Get<Vector2>();

            // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
            if (inputBuffer != Vector3.zero)
            {
                if(inputBuffer.y != 0f)
                    inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

                movementDirection = inputBuffer;
            }
            else
            {
                ChangeToIdleStateOnStoppingMovement(inputBuffer);
            }
        }        
    }

    private void ChangeToIdleStateOnStoppingMovement(Vector3 inputBuffer)
    {
        if (inputBuffer == Vector3.zero)
        {
            idleState.enabled = true;
            this.enabled = false;
        }
    }

    private void OnCrouch(InputValue inputValue)
    {
        ChangeToCrouchingStateOnCrouchButtonPress(inputValue);
    }

    private void ChangeToCrouchingStateOnCrouchButtonPress(InputValue inputValue)
    {
        if (this.enabled &&
            IsCrouchButtonPressed(inputValue))
        {
            crouchingState.enabled = true;
            this.enabled = false;
        }
    }

    private static bool IsCrouchButtonPressed(InputValue inputValue)
    {
        return !Mathf.Approximately(inputValue.Get<float>(), 0f);
    }
}
