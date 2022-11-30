using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(CharacterMovement))]
public class CharacterInput : MonoBehaviour
{   public bool IsWalking { get; private set; }
    public bool IsCrouching { get; private set; }

    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();

        // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
        if (inputBuffer.y != 0)
            inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

        characterMovement.SetMovementDirection(inputBuffer);
    }


    // TODO: tratar de que caminar (y quizá agacharse también) funcione manteniendo el botón apretado y que al dejar de apretar se vuelva a correr.
    private void OnWalk()
    {
        //IsWalking = !IsWalking;
        //walkEvent.Invoke();
    }

    private void OnCrouch()
    {
        //foreach(UnityEvent e in crouchEvent)
        //{
        //    e.Invoke();
        //}
        //IsWalking = !IsWalking; // TODO: needed so CharacterMovement applies right speed, change code to actually look at this.
        //IsCrouching = !IsCrouching; 
    }
}
