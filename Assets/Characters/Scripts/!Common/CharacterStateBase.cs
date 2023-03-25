using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class CharacterStateBase : MonoBehaviour
{
    private static MovementProperties movementProperties = new MovementProperties();

    [HideInInspector] public UnityEvent<Vector3> onMovementSpeedChange;

    [Header("Movement Characteristics")]
    [SerializeField] protected float speed = 6f;
    public float Speed => speed;
  
    private Collider[] targets;
    private static Transform target;
    private static Collider targetCol;
    private static Transform cameraFollowTarget;
    private LayerMask soldierLayer = 1 << 9;
    private LayerMask obstacleLayer = ~(1 << 3);

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
        if (!target || !targetCol.enabled)
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
            target?.GetComponent<TrackedObject>()?.SetIsIndicatorVisible(false);
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

    private static Coroutine cameraSwitchingCoroutine;
    private void OnFocus()
    {
        if (cameraSwitchingCoroutine == null)
        {
            cameraSwitchingCoroutine = StartCoroutine(ChangeFocus());
        }
    }

    private IEnumerator ChangeFocus()
    {
        SwitchCameras();
       
        yield return new WaitForSeconds(0.5f);
        
        StopCoroutine(cameraSwitchingCoroutine);
        cameraSwitchingCoroutine = null;
    }

    private void SwitchCameras()
    {
        if (!isFocusedOnEnemy)
        {
            targets = Physics.OverlapSphere(transform.position, 10f, soldierLayer);
            FilterTargetsByDistanceToPlayer();

            if (target)
            {
                unfocusedCamera.gameObject.SetActive(false);
                focusedCamera.gameObject.SetActive(true);

                target.GetComponent<TrackedObject>()?.SetIsIndicatorVisible(true); // Only happens in one frame
            }
        }
        else
        {
            focusedCamera.gameObject.SetActive(false);
            unfocusedCamera.gameObject.SetActive(true);

            target?.GetComponent<TrackedObject>()?.SetIsIndicatorVisible(false);

            targets = null;
            target = null;
        }

        isFocusedOnEnemy = !isFocusedOnEnemy;
    }

    private void FilterTargetsByDistanceToPlayer()
    {
        if(targets.Length < 1) { return; }
        
        for(int i = 1; i < targets.Length; i++)
        {
            if ((targets[i].transform.position - transform.position).sqrMagnitude < (targets[i - 1].transform.position - transform.position).sqrMagnitude)
            {
                Collider temp = targets[i - 1];
                targets[i - 1] = targets[i];
                targets[i] = temp;
                i--;
            }
        }

        foreach(Collider item in targets)
        {
            if (IsTargetNotObstructed(item))
            {
                target = item.transform;
                targetCol = item;
                break;
            }
        }
    }

    private bool IsTargetNotObstructed(Collider item)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, (item.transform.position - transform.position).normalized, out hit, 11f, obstacleLayer);

        //Debug.DrawLine(transform.position, hit.point, Color.red, 10f);
        
        if (hit.collider.CompareTag("Soldier"))
        {
            return true;
        }
        return false;
    }

    private bool IsCurrentItemCloserThanCurrentTarget(Collider item)
    {
        return (item.transform.position - transform.position).sqrMagnitude < (target.position - transform.position).sqrMagnitude;
    }
}
