using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{    
    public Vector3 MovementDirection { get; private set; }

    private void OnMove(InputValue inputValue)
    {
        Vector3 input = inputValue.Get<Vector2>().normalized;

        switch (input)
        {
            case Vector3 i when i == Vector3.up:
                MovementDirection = Vector3.forward;
                break;
            case Vector3 i when i == Vector3.down:
                MovementDirection = Vector3.back;
                break;
            case Vector3 i when i == Vector3.right:
                MovementDirection = Vector3.right;
                break;
            case Vector3 i when i == Vector3.left:
                MovementDirection = Vector3.left;
                break;
            default:
                MovementDirection = Vector3.zero;
                break;
        }
    }
}
