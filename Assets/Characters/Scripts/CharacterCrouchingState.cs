using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterRunningState), typeof(CharacterIdleState))]
[RequireComponent(typeof(CharacterOnWallState), typeof(CharacterOnAirState))]
public class CharacterCrouchingState : CharacterMovementBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterRunningState runningState;
    [SerializeField] CharacterIdleState idleState;
    [SerializeField] CharacterOnWallState onWallState;
    [SerializeField] CharacterOnAirState onAirState;

    [HideInInspector] public UnityEvent attachCharacterToWall;

    private void Awake()
    {
        this.enabled = false;
    }

    void Update()
    {
        if (!charController.isGrounded)
            ChangeToOnAirState();

        UpdateMovement(speed, movementDirection, Vector3.up);

        if(movementDirection != Vector3.zero)
            OrientateCharacterForward();
    }

    private void ChangeToOnAirState()
    {
        onAirState.enabled = true;
        this.enabled = false;
    }

    // TODO: refactor this OnMove repeated code from CharacterRunningState, it could go on CharacterMovementBase
    void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            Vector3 inputBuffer = inputValue.Get<Vector2>();

            // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
            if (inputBuffer != Vector3.zero)
            {
                if (inputBuffer.y != 0f)
                    inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

                movementDirection = inputBuffer;
            }
            else
            {
                movementDirection = Vector3.zero;
            }
        }
    }


    private void OnCrouch(InputValue inputValue)
    {
        ChangeStateOnCrouchButtonRelease(inputValue);
    }

    private void ChangeStateOnCrouchButtonRelease(InputValue inputValue)
    {
        if (this.enabled &&
            IsCrouchButtonReleased(inputValue))
        {
            if(movementDirection != Vector3.zero)
            {
                runningState.enabled = true;
                this.enabled = false;
            }
            else
            {
                idleState.enabled = true;
                this.enabled = false;
            }
            
        }
    }

    private static bool IsCrouchButtonReleased(InputValue inputValue)
    {
        return Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (this.enabled && hit.collider.CompareTag("Cover"))
        {
            attachCharacterToWall.Invoke();

            onWallState.SetNormalToWallPlane(hit.normal);
            onWallState.enabled = true;
            this.enabled = false;
        }
    }
}
