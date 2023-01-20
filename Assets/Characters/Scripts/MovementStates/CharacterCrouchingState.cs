using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterCrouchingState : CharacterStateBase
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

        if(movementDirection != Vector3.zero)
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
