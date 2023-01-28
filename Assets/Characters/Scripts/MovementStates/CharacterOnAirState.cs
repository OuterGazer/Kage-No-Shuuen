using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterOnAirState : CharacterStateBase
{
    [Header("State Parameters")]
    [SerializeField] float groundDistanceThreshold = 1.0f;

    [HideInInspector] public UnityEvent<bool> IsCharacterTouchingGround;
    [HideInInspector] public UnityEvent ChangeToLandingAnimation;

    private bool hasCharacterLanded = false;

    private void Awake()
    {
        IsCharacterTouchingGround.Invoke(true);
    }

    private void OnEnable()
    {
        transform.up = Vector3.up;
        hasCharacterLanded = false;
        IsCharacterTouchingGround.Invoke(false);
    }

    private void Update()
    {
        UpdateMovement(speed, Vector3.zero, Vector3.up);

        if(charController.isGrounded)
        {
            ExitState();
        }
    }

    private void ExitState()
    {
        IsCharacterTouchingGround.Invoke(true);
    }

    private void FixedUpdate()
    {
        if (!hasCharacterLanded && IsCharacterAboutToLand())
        {
            ChangeToLandingAnimation.Invoke();
            hasCharacterLanded = true;
        }            
    }

    bool IsCharacterAboutToLand()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistanceThreshold);
    }
}
