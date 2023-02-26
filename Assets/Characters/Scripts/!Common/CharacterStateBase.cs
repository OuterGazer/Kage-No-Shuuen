using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterStateBase : MonoBehaviour
{
    private static MovementProperties movementProperties = new MovementProperties();

    [HideInInspector] public UnityEvent<Vector3> onMovementSpeedChange;

    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;

    private static Camera mainCamera;
    protected static CharacterController charController;

    protected static Vector3 movementDirection;
    public static Vector3 MovementDirection => movementDirection;

    //private static float velocityY = 0f; // No se usa

    protected static float movingSpeed;

    protected void UpdateMovement(float speed, Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        movementProperties.UpdateMovement(speed, movementDirection, movementProjectionPlane);
    }

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        charController = characterController;

        movementProperties.MainCamera = mainCamera;
        movementProperties.CharController = charController;
    }

    private static float timeToOrientateCharacterForward = 0.25f;
    protected void OrientateCharacterForward()
    {
        Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

        DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

        // Alternativa sin DoTween pero con un fallo
        // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
        //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
        //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
        //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
    }

    protected void PushCharacterForward(float stepForwardLength)
    {
        charController.Move(stepForwardLength * Time.deltaTime * transform.forward);
    }

    private void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();

        // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
        if (inputBuffer != Vector3.zero)
        {
            if (inputBuffer.y != 0f)
                inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

            movementDirection = inputBuffer;

            onMovementSpeedChange.Invoke(movementDirection); //Tells CharacterAnimator current direction of movement
        }
        else
        {
            movementDirection = Vector3.zero;
        }
    }
}
