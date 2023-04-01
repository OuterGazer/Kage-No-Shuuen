using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDeadState : CharacterStateBase
{
    private void OnEnable()
    {
        if (charController)
        { 
            charController.detectCollisions = false;
            gameObject.layer = 0;
        }
    }


    private void Update()
    {
        if (charController.enabled)
        { UpdateMovement(speed, movementDirection, Vector3.up); }
    }

    private void OnDisable()
    {
        SetUnfocusedCameraOnDeath();
    }
}
