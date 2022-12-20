using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterCrouchingState), typeof(CharacterRunningState), typeof(CharacterOnHookState))]
[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class CharacterIdleState : CharacterMovementBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterRunningState runningState;
    [SerializeField] CharacterCrouchingState crouchingState;
    [SerializeField] CharacterOnHookState onHookState;

    private void Awake()
    {
        this.enabled = true;
        SetCameraAndCharController(GetComponent<CharacterController>());
    }

    private void OnEnable()
    {
        movementDirection = Vector3.zero;            
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);
    }


    public void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            if (inputValue.Get<Vector2>() != Vector2.zero)
            {
                runningState.enabled = true;
                this.enabled = false;
            }
        }
    }

    public void OnCrouch(InputValue inputValue)
    {
        if (this.enabled)
        {
            crouchingState.enabled = true;
            this.enabled = false;
        }
    }

    public void OnHookThrow()
    {
        if (this.enabled)
        {
            onHookState.enabled = true;
            this.enabled = false;
        }
    }

    public void OnBlock(InputValue inputValue)
    {
        if (this.enabled)
        {
            float temp = inputValue.Get<float>();

            if(temp > 0f)
                BroadcastMessage("UpdateBlocking", true);
            else
                BroadcastMessage("UpdateBlocking", false);
        }
    }
}
