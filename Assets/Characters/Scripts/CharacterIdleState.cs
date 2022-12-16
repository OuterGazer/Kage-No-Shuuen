using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterRunningState))]
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
        // TODO: encontrar una manera programática de agregar un target (con un Overlap/CheckSphere?)
        // TODO: pensar en si quiero que el jugador pueda lanzar el gancho estando OnWall
        if (this.enabled)
        {
            onHookState.enabled = true;
            this.enabled = false;
        }
    }
}
