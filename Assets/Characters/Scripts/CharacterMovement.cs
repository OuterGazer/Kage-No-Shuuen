using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] InputAction moveForward;
    [SerializeField] InputAction moveBackward;
    [SerializeField] InputAction strafeLeft;
    [SerializeField] InputAction strafeRight;
    [SerializeField] float speed = 3f;

    private CharacterController characterController;

    private void Awake()
    {
        moveForward.Enable();
        moveBackward.Enable();
        strafeLeft.Enable();
        strafeRight.Enable();

        characterController = GetComponent<CharacterController>();
    }

    private void OnDestroy()
    {
        moveForward.Disable();
        moveBackward.Disable();
        strafeLeft.Disable();
        strafeRight.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: revisar si realmente necesito el Time.deltaTime para mover character controller
        if(moveForward.IsPressed())
            characterController.Move(Vector3.forward * speed * Time.deltaTime);

        if(moveBackward.IsPressed())
            characterController.Move(-Vector3.forward * speed * Time.deltaTime);

        if (strafeLeft.IsPressed())
            characterController.Move(-Vector3.right * speed * Time.deltaTime);

        if (strafeRight.IsPressed())
            characterController.Move(Vector3.right * speed * Time.deltaTime);
    }
}
