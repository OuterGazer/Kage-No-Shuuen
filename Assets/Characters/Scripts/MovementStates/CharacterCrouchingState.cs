using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterCrouchingState : CharacterStateBase
{
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
