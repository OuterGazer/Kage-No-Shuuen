using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterRunningState))]
public class CharacterIdleState : CharacterMovementBase
{
    [Header("Exit Scripts")]
    [SerializeField] CharacterRunningState runningState;

    private void Awake()
    {
        this.enabled = true;
        SetCameraAndCharController(GetComponent<CharacterController>());
    }

    private void Update()
    {
        UpdateMovement(speed, Vector3.zero);
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
}
