using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterIdleState : CharacterStateBase
{
    public UnityEvent<bool> onIdle;

    private void Awake()
    {
        SetCameraAndCharController(GetComponent<CharacterController>());
    }

    private void OnEnable()
    {
        onIdle.Invoke(true);
    }

    private void OnDisable()
    {
        onIdle.Invoke(false);
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);
    }
}
