using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRunningState : CharacterStateBase
{
    private void Awake()
    {
        this.enabled = false;
    }

    void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

        OrientateCharacterForward();
    }
}
