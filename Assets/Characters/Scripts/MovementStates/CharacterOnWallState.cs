using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using DG.Tweening;

public class CharacterOnWallState : CharacterStateBase
{
    [HideInInspector] public UnityEvent AttachCharacterToWall;
    [HideInInspector] public UnityEvent RemoveCharacterFromWall;
    [HideInInspector] public UnityEvent<float> CorrectCharacterAnimationWhenCameraIsNearWall;
    private LayerMask coverMask;

    private static Vector3 normalToWallPlane;
    private float charControllerHorizontalBound;

    private void Start()
    {
        coverMask = LayerMask.GetMask("Cover");
        charControllerHorizontalBound = GetComponent<CharacterController>().bounds.extents.x;
    }

    private void OnEnable()
    {
        AttachCharacterToWall.Invoke();
        SetNormalToWallPlane(Vector3.forward);
    }

    private void OnDisable()
    {
        RemoveCharacterFromWall.Invoke();
    }

    [SerializeField] float timeToCompleteCharacterOrientation = 0.25f;
    private void Update()
    {
        TurnCharacterAgainstTheWall();

        UpdateMovement(speed, movementDirection, transform.forward);

        ApplyCorrectAnimation();
    }

    
    private void TurnCharacterAgainstTheWall()
    {
        if (IsCharacterBackNotAgainstTheWall())
        {
            DOTween.To(() => transform.forward, x => transform.forward = x, normalToWallPlane, timeToCompleteCharacterOrientation);
        }
    }

    public static void SetNormalToWallPlane(Vector3 normalPlane)
    {
        normalToWallPlane = normalPlane;
    }

    private bool IsCharacterBackNotAgainstTheWall()
    {
        return !Mathf.Approximately(Vector3.Dot(transform.forward, normalToWallPlane), 1f);
    }

    private void ApplyCorrectAnimation()
    {
        if (IsPlayerOnWallAndUsingWSInsteadOfAD())
        {
            if (IsPlayerForwardPointingTheSameDirectionAsCameraRight())
                CorrectCharacterAnimationWhenCameraIsNearWall.Invoke(+1f);
            else
                CorrectCharacterAnimationWhenCameraIsNearWall.Invoke(-1f);
        }
    }

    private bool IsPlayerOnWallAndUsingWSInsteadOfAD()
    {
        return IsCameraNearWall();
    }

    [SerializeField] float cameraThresholdNearWall = 0.85f;
    private bool IsCameraNearWall()
    {
        float dotProductCameraForwardAndPlayerXAxis = Vector3.Dot(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up), transform.right);

        if (dotProductCameraForwardAndPlayerXAxis >= cameraThresholdNearWall ||
            dotProductCameraForwardAndPlayerXAxis <= -cameraThresholdNearWall)
        { return true; }
        else
        { return false; }
    }

    private bool IsPlayerForwardPointingTheSameDirectionAsCameraRight()
    {
        Vector3 projectedCameraRightToUpPlane = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);
        return Vector3.Dot(projectedCameraRightToUpPlane, transform.forward) >= cameraThresholdNearWall;
    }

    private void FixedUpdate()
    {
        if (HasCoverEdgeBeenReached())
            { DoNotAllowCharacterToMovePastEdge(); }
    }

    private void DoNotAllowCharacterToMovePastEdge()
    {
        movementDirection = Vector3.zero;
        movingSpeed = 0f;
    }

    [SerializeField] float stoppingDistanceToCoverEdge = 3.75f;
    private bool HasCoverEdgeBeenReached()
    {
        float characterMovementDirectionOnWall = Mathf.Sign(Vector3.Dot(transform.right, currentHorizontalMovement));

        Vector3 raycastOriginPoint = transform.position + charControllerHorizontalBound * characterMovementDirectionOnWall *
                                                          stoppingDistanceToCoverEdge * transform.right;

        return !Physics.Raycast(raycastOriginPoint, -transform.forward, 0.5f, coverMask);
    }
}
