using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterIdleState), typeof(CharacterOnAirState))]
public class CharacterCrouchingState : CharacterStateBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterIdleState idleState;
    [SerializeField] CharacterOnAirState onAirState;

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

        if(movementDirection != Vector3.zero)
            OrientateCharacterForward();

        if (!charController.isGrounded)
            ChangeToOnAirState();
    }

    private void ChangeToOnAirState()
    {
        onAirState.enabled = true;
        this.enabled = false;

        idleState.move.Disable();
    }
}
