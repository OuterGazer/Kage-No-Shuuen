using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;

public class CharacterStateBase : MonoBehaviour
{
    private static MovementProperties movementProperties = new MovementProperties();

    [HideInInspector] public UnityEvent<Vector3> onMovementSpeedChange;

    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;
    private Collider[] targets;
    private static Transform target;
    private LayerMask testLayer = 1 << 9;

    private static Camera mainCamera;
    private static CinemachineFreeLook freeLookCamera;
    protected static CharacterController charController;

    protected static Vector3 movementDirection;
    public static Vector3 MovementDirection => movementDirection;

    protected static float movingSpeed;

    private static bool isFocusedOnEnemy = false;

    protected void UpdateMovement(float speed, Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        movementProperties.UpdateMovement(speed, movementDirection, movementProjectionPlane);
    }

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        charController = characterController;
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        movementProperties.MainCamera = mainCamera;
        movementProperties.CharController = charController;
    }

    private static float timeToOrientateCharacterForward = 0.25f;
    protected void OrientateCharacterForward()
    {
        if (!target)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

            CorrectCameraBindingModeIfFocusedOnEnemyButThereIsNoTarget();
        }
        else
        {
            Vector3 targetDirectionNormalized = (target.position - transform.position).normalized;
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(targetDirectionNormalized, Vector3.up);
            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);
        }
        // Alternativa sin DoTween pero con un fallo
        // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
        //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
        //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
        //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
    }

    private static void CorrectCameraBindingModeIfFocusedOnEnemyButThereIsNoTarget()
    {
        if (isFocusedOnEnemy)
        {
            isFocusedOnEnemy = !isFocusedOnEnemy;
            freeLookCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        }
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

    private void OnFocus()
    {
        isFocusedOnEnemy = !isFocusedOnEnemy;

        if (isFocusedOnEnemy)
        {
            targets = Physics.OverlapSphere(transform.position, 10f, testLayer);
            foreach(Collider item in targets)
            {
                if ((item.transform.position - transform.position).sqrMagnitude < (target.position - transform.position).sqrMagnitude)
                {
                    target = item.transform;
                }
            }
            freeLookCamera.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
        }
        else
        {
            targets = null;
            target = null;
            freeLookCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        }
    }
}
