using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{    
    public Vector3 MovementDirection { get; private set; }
    public bool IsWalking { get; private set; }
    public bool IsCrouching { get; private set; }

    [SerializeField] UnityEvent crouchEvent;

    private void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();


        // Movement from Input Module sends only up and down movement and it needs to be corrected into forward and backward.
        if (inputBuffer.y != 0)
            inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

        MovementDirection = inputBuffer;
    }


    // TODO: tratar de que caminar funcione manteniendo el botón apretado y que al dejar de apretar se vuelva a correr.
    private void OnWalk()
    {
        IsWalking = !IsWalking;
    }

    private void OnCrouch()
    {
        IsWalking = !IsWalking; // TODO: needed so CharacterMovement applies right speed, change code to actually look at this.
        IsCrouching = !IsCrouching; 
    }
}
