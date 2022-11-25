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

    private float velocityY;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterInput = GetComponent<CharacterInput>();

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalMovement = UpdateHorizontalMovement() * speed * Time.deltaTime;
        Vector3 verticalMovement = UpdateVerticalMovement();
        characterController.Move(horizontalMovement + verticalMovement);
    }

    private Vector3 UpdateVerticalMovement()
    {
        velocityY = Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        { velocityY = 0; }

        return new Vector3(0, velocityY, 0);
    }

    private Vector3 UpdateHorizontalMovement()
    {
        Vector3 movement = ApplyMovementRelativeToCameraPosition();

        MakeCharacterAlwaysFaceForwardOnMovement(movement);

        return movement;       
    }

    private void MakeCharacterAlwaysFaceForwardOnMovement(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);            
            transform.rotation = Quaternion.LookRotation(projectedForwardVector, Vector3.up);
        }
    }

    private Vector3 ApplyMovementRelativeToCameraPosition()
    {
        Vector3 movement = mainCamera.transform.TransformDirection(characterInput.MovementDirection);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up);
        return movement;
    }
}
