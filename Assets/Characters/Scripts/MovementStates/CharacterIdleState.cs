using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class CharacterIdleState : CharacterStateBase
{
    private PlayerInput playerInput;
    public InputAction move;

    private bool isMovingAfterDodging = true;

    private void Awake()
    {
        SetCameraAndCharController(GetComponent<CharacterController>());        
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        onMovementStateChange.Invoke(this);

        // If I'm setting movementDirection to zero direct in the OnMove() in the parent
        // when there is no movement, I probably don't need to do it here as well

        //movementDirection = Vector3.zero;          
    }

    private void OnDisable()
    {
        this.isMovingAfterDodging = true;
    }

    // Called from CharacterDodgingState OnDisable
    public void EnableMovement()
    {
        if (!move.enabled)
        {
            move.Enable();

            isMovingAfterDodging = false;

            StartCoroutine(AccelerateDirectionChange());
        }
    }

    // TODO: substitute all these magic numbers
    private IEnumerator AccelerateDirectionChange()
    {
        accMovementDir = 20f;

        yield return new WaitForSeconds(0.25f);

        accMovementDir = 1.5f;
    }

    private void Update()
    {
        if (!isMovingAfterDodging)
            movingSpeed = 0f;

        UpdateMovement(speed, movementDirection, Vector3.up);
    }
}
