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
public class CharacterIdleState : CharacterStateBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterRunningState runningState;
    [SerializeField] CharacterCrouchingState crouchingState;
    [SerializeField] CharacterOnHookState onHookState;

    private PlayerInput playerInput;
    public InputAction move;

    private bool isMovingAfterDodging = true;
    //public bool IsMovingAfterDodging { 
    //    get { return isMovingAfterDodging;}
    //    set { isMovingAfterDodging = value; }
    //}

    private void Awake()
    {
        this.enabled = true;
        SetCameraAndCharController(GetComponent<CharacterController>());
        
        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        onMovementStateChange.Invoke(this);

        movementDirection = Vector3.zero;          
    }

    private void OnDisable()
    {
        //this.isMovingAfterDodging = true;
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
        //if (!isMovingAfterDodging)
        //    movingSpeed = 0f;

        UpdateMovement(speed, movementDirection, Vector3.up);
    }

    //public void OnCrouch()
    //{
    //    if (this.enabled)
    //    {
    //        crouchingState.enabled = true;
    //        this.enabled = false;
    //    }
    //}

    public void OnHookThrow()
    {
        if (this.enabled)
        {
            onHookState.enabled = true;
            this.enabled = false;
        }
    }
}
