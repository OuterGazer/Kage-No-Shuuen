using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHitState : CharacterStateBase
{
    private void OnEnable()
    {
        if (charController)
        { charController.detectCollisions = false; }
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);
    }
}
