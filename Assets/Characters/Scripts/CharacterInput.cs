using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{    public Vector3 movementDirection { get; private set; }

    private void OnMoveForward()
    {
        movementDirection = Vector3.forward;
    }

    private void OnMoveBackward()
    {
        movementDirection = -Vector3.forward;
    }

    private void OnStrafeLeft()
    {
        movementDirection = -Vector3.right;
    }

    private void OnStrafeRight()
    {
        movementDirection = Vector3.right;
    }
}
