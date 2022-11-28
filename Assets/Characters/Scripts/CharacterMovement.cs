using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(CharacterInput), typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    // Due to how animation in the blend tree works, walking speed must be always half of running speed.
    // TODO: look on a way to decouple the previous dependency.
    [SerializeField] float runningSpeed = 10f;
    [SerializeField] float walkingSpeed = 3f;
    [SerializeField] float crouchingSpeed = 3f;
    public float RunningSpeed => runningSpeed;
    public float WalkingSpeed => walkingSpeed;
    public float CrouchingSpeed => crouchingSpeed;

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

    float movingSpeed;
    // Update is called once per frame
    void Update()
    {
        // TODO: add case for crouching (right now crouching and walking have same speed and thus the code works) Switch Statement and enum        
        if (characterInput.IsWalking)
        {
            movingSpeed = Mathf.MoveTowards(movingSpeed, walkingSpeed, 0.05f);
            //movingSpeed = walkingSpeed;
        }
        else
        {
            movingSpeed = Mathf.MoveTowards(movingSpeed, runningSpeed, 0.05f);
            //movingSpeed = runningSpeed;
        }

        //DOTween.To(() => transform.position, x => transform.position = x, new Vector3(2, 2, 2), 1);        

        Vector3 horizontalMovement = UpdateHorizontalMovement() * movingSpeed * Time.deltaTime;
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
