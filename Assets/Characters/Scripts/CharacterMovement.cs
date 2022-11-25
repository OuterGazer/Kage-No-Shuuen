using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterInput), typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float speed = 3f;

    private CharacterController characterController;
    private CharacterInput characterInput;
    private Camera mainCamera;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterInput = GetComponent<CharacterInput>();

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHorizontalMovement();
    }

    private void UpdateHorizontalMovement()
    {
        Vector3 movement = ApplyMovementRelativeToCameraPosition();

        characterController.Move(movement * speed * Time.deltaTime);

        MakeCharacterAlwaysFaceForwardOnMovement(movement);
    }

    private void MakeCharacterAlwaysFaceForwardOnMovement(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            transform.forward = projectedForwardVector;
        }
    }

    private Vector3 ApplyMovementRelativeToCameraPosition()
    {
        Vector3 movement = mainCamera.transform.TransformDirection(characterInput.MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }
}
