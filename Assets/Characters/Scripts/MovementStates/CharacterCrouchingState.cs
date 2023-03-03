using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterCrouchingState : CharacterStateBase
{
    public UnityEvent<bool> onCrouch;

    private void OnEnable()
    {
        onCrouch.Invoke(true);
    }

    private void OnDisable()
    {
        onCrouch.Invoke(false);
    }

    void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);        
    }

    private void LateUpdate()
    {
        if (movementDirection != Vector3.zero)
            OrientateCharacterForward();
    }
}
