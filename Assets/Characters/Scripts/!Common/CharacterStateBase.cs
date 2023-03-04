using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting.Dependencies.NCalc;

public class CharacterStateBase : MonoBehaviour
{
    private static MovementProperties movementProperties = new MovementProperties();

    [HideInInspector] public UnityEvent<Vector3> onMovementSpeedChange;

    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;
  
    private Collider[] targets;
    private static Transform target;
    private static Transform cameraFollowTarget;
    private LayerMask soldierLayer = 1 << 9;

    private static Camera mainCamera;
    private static CinemachineFreeLook unfocusedCamera;
    private static CinemachineFreeLook focusedCamera;
    protected static CharacterController charController;

    protected static Vector3 movementDirection;
    public static Vector3 MovementDirection => movementDirection;

    protected static float movingSpeed;

    private static bool isFocusedOnEnemy = false;

    protected void PushCharacterForward(float stepForwardLength)
    {
        Vector3 pushForward = Vector3.zero;
        pushForward = stepForwardLength * Time.deltaTime * transform.forward;
        pushForward.y = -0.1f;

        charController.Move(pushForward);
    }

    protected void UpdateMovement(float speed, Vector3 movementDirection, Vector3 movementProjectionPlane)
    {
        movementProperties.UpdateMovement(speed, movementDirection, movementProjectionPlane);

        OrientateCameraFollowTarget();
    }

    protected void OrientateCameraFollowTarget()
    {
        if (!isFocusedOnEnemy)
        {
            cameraFollowTarget.forward = transform.forward;
        }
        else
        {
            cameraFollowTarget.LookAt(target, Vector3.up);
        }
    }

    protected void SetCameraAndCharController(CharacterController characterController)
    {
        mainCamera = Camera.main;
        charController = characterController;

        movementProperties.MainCamera = mainCamera;
        movementProperties.CharController = charController;

        GetReferencesToSceneCameras();

        cameraFollowTarget = focusedCamera.Follow;
        focusedCamera.gameObject.SetActive(false);
    }

    private static void GetReferencesToSceneCameras()
    {
        CinemachineFreeLook[] freeLookCameras = FindObjectsOfType<CinemachineFreeLook>();

        foreach (CinemachineFreeLook item in freeLookCameras)
        {
            if (item.CompareTag("CamFocused"))
            {
                focusedCamera = item;
            }
            else if (item.CompareTag("CamUnfocused"))
            {
                unfocusedCamera = item;
            }
        }
    }

    private static float timeToOrientateCharacterForward = 0.25f;
    protected void OrientateCharacterForward()
    {
        Vector3 projectedForwardVector = Vector3.zero;
        if (!target)
        {
            projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

            CorrectCameraBindingModeIfFocusedOnEnemyButThereIsNoTarget();
        }
        else
        {
            OrientateCharacterTowardsTarget(projectedForwardVector);
        }
        // Alternativa sin DoTween pero con un fallo
        // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
        //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
        //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
        //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
    }

    private void CorrectCameraBindingModeIfFocusedOnEnemyButThereIsNoTarget()
    {
        if (isFocusedOnEnemy)
        {
            isFocusedOnEnemy = !isFocusedOnEnemy;
            unfocusedCamera.gameObject.SetActive(true);
            focusedCamera.gameObject.SetActive(false);
            targets = null;
            target = null;
        }
    }

    private void OrientateCharacterTowardsTarget(Vector3 projectedForwardVector)
    {
        Vector3 targetDirectionNormalized = (target.position - transform.position).normalized;
        projectedForwardVector = Vector3.ProjectOnPlane(targetDirectionNormalized, Vector3.up);
        DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);
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

    private bool hasFocusChangedLastFrame = false;
    private static Coroutine test;
    private void OnFocus()
    {
        if (!hasFocusChangedLastFrame && !isFocusedOnEnemy)
        {
            //Debug.Log("focusing!");
            isFocusedOnEnemy = true;
            hasFocusChangedLastFrame = true;

            //Debug.Log("Focus!");
            targets = Physics.OverlapSphere(transform.position, 10f, soldierLayer);
            FilterTargetsByDistanceToPlayer();
            unfocusedCamera.gameObject.SetActive(false);
            focusedCamera.gameObject.SetActive(true);
        }
        else if (!hasFocusChangedLastFrame && isFocusedOnEnemy)
        {
            //Debug.Log("unfocusing!");
            isFocusedOnEnemy = false;
            hasFocusChangedLastFrame = true;

            //Debug.Log("unfocus!");
            focusedCamera.gameObject.SetActive(false);
            unfocusedCamera.gameObject.SetActive(true);
            targets = null;
            target = null;
        }

        if (test == null)
        {
            test = StartCoroutine(ChangeFocus());
        }
    }

    private IEnumerator ChangeFocus()
    {
        Debug.Log("Here");
        yield return new WaitForSeconds(0.5f);
        hasFocusChangedLastFrame = false;
        Debug.Log("here again");
    }

    private void FilterTargetsByDistanceToPlayer()
    {
        foreach (Collider item in targets)
        {
            if (!target)
            {
                target = item.transform;
            }
            else if (IsCurrentItemCloserThanCurrentTarget(item))
            {
                target = item.transform;
            }
        }
    }

    private bool IsCurrentItemCloserThanCurrentTarget(Collider item)
    {
        return (item.transform.position - transform.position).sqrMagnitude < (target.position - transform.position).sqrMagnitude;
    }
}
