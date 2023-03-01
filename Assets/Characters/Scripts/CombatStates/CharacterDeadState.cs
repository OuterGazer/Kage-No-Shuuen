using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDeadState : CharacterStateBase
{
    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);
    }
}
