using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterCrouchingState), typeof(CharacterRunningState), typeof(CharacterOnHookState))]
[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class CharacterIdleState : CharacterMovementBase
{
    [SerializeField] Rig blockingRig;
    private bool isBlocking = false;

    [Header("Exit Scripts")]
    [SerializeField] CharacterRunningState runningState;
    [SerializeField] CharacterCrouchingState crouchingState;
    [SerializeField] CharacterOnHookState onHookState;

    private PlayerInput playerInput;
    public InputAction move;

    private void Awake()
    {
        this.enabled = true;
        SetCameraAndCharController(GetComponent<CharacterController>());
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        movementDirection = Vector3.zero;

        if (!move.enabled)
            move.Enable();
    }

    private void Update()
    {
        

        UpdateMovement(speed, movementDirection, Vector3.up);

        ChangeBlockingRiggingWeight();
    }

    [SerializeField] float weightChangeAcceleration = 1f;
    private void ChangeBlockingRiggingWeight()
    {
        if (!isBlocking)
        {
            if (blockingRig.weight >= 0f)
                blockingRig.weight -= weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 0f;
        }
        else
        {
            if (blockingRig.weight <= 1f)
                blockingRig.weight += weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 1f;
        }

        
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

            move.Disable();
        }
    }


    // TODO: mover lento al personaje mientras bloquea, velocidad de agachado o menos.
    public void OnBlock(InputValue inputValue)
    {
        if (this.enabled)
        {
            float temp = inputValue.Get<float>();

            //TODO: Refactor all the rigging stuff into own script and change BroadcastMessage for proper events (also look in update!)
            if(temp > 0f)
            {
                BroadcastMessage("UpdateBlocking", true);
                isBlocking = true;
            }

            else
            {
                BroadcastMessage("UpdateBlocking", false);
                isBlocking = false;
            }
        }
    }
}
