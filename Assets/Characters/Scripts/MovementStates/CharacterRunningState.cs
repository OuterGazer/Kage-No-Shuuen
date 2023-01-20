using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterRunningState : CharacterStateBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterIdleState idleState;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        onMovementStateChange.Invoke(this);
    }

    void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

        OrientateCharacterForward();

        if (!charController.isGrounded)
            ChangeToOnAirState();
    }

    private void ChangeToOnAirState()
    {
        onBeingOnAir.Invoke();
        idleState.move.Disable();
    }
}
