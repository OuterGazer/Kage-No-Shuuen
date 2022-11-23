using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    [SerializeField] InputAction moveForward;
    [SerializeField] InputAction moveBackward;
    [SerializeField] InputAction strafeLeft;
    [SerializeField] InputAction strafeRight;

    public Vector3 movementDirection { get; private set; }

    private void Awake()
    {
        moveForward.Enable();
        moveBackward.Enable();
        strafeLeft.Enable();
        strafeRight.Enable();
    }

    private void OnDestroy()
    {
        moveForward.Disable();
        moveBackward.Disable();
        strafeLeft.Disable();
        strafeRight.Disable();
    }

    private void Update()
    {
        movementDirection = Vector3.zero;

        if (moveForward.IsPressed())
            movementDirection = Vector3.forward;

        if (moveBackward.IsPressed())
            movementDirection = -Vector3.forward;

        if (strafeLeft.IsPressed())
            movementDirection = -Vector3.right;

        if (strafeRight.IsPressed())
            movementDirection = Vector3.right;
    }
}
