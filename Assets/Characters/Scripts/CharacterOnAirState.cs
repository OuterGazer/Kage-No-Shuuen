using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterOnAirState : CharacterMovementBase
{
    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [Header("State Parameters")]
    [SerializeField] float groundDistanceThreshold = 1.0f;

    [HideInInspector] public UnityEvent<bool> isCharacterTouchingGround;
    [HideInInspector] public UnityEvent changeToLandingAnimation;

    private bool hasCharacterLanded = false;

    private void Awake()
    {
        isCharacterTouchingGround.Invoke(true);
        this.enabled = false;
    }

    private void OnEnable()
    {
        transform.up = Vector3.up;
        hasCharacterLanded = false;
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

    private void FixedUpdate()
    {
        if (!hasCharacterLanded && IsCharacterAboutToLand())
        {
            changeToLandingAnimation.Invoke();
            hasCharacterLanded = true;
        }            
    }

    bool IsCharacterAboutToLand()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistanceThreshold);
    }
}
