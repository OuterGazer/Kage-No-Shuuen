using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterOnAirState : CharacterMovementBase
{
    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [HideInInspector] public UnityEvent<bool> isCharacterTouchingGround;

    private void Awake()
    {
        isCharacterTouchingGround.Invoke(true);
        this.enabled = false;
    }

    private void OnEnable()
    {
        transform.up = Vector3.up;
        isCharacterTouchingGround.Invoke(false);
    }

    private void Update()
    {
        UpdateMovement(speed, Vector3.zero, Vector3.up);

        if(charController.isGrounded)
        {
            isCharacterTouchingGround.Invoke(true);
            idleState.enabled = true;
            this.enabled = false;
        }
    }
}
