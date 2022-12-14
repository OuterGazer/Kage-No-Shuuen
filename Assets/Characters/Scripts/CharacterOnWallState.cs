using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using DG.Tweening;

public class CharacterOnWallState : CharacterMovementBase
{
    [Header("Exit States")]
    [SerializeField] CharacterCrouchingState crouchingState;

    [HideInInspector] public UnityEvent removeCharacterFromWall;
    [HideInInspector] public UnityEvent<float> correctCharacterAnimationWhenCameraIsNearWall;
    private LayerMask coverMask;

    private Vector3 normalToWallPlane;
    private float charControllerHorizontalBound;

    private void Awake()
    {
        this.enabled = false;
    }

    private void Start()
    {
        coverMask = LayerMask.GetMask("Cover");
        charControllerHorizontalBound = GetComponent<CharacterController>().bounds.extents.x;
    }

    [SerializeField] float timeToCompleteCharacterOrientation = 0.25f;
    private void Update()
    {
        TurnCharacterAgainstTheWall();

        UpdateMovement(speed, movementDirection, transform.forward);
    }

    private void TurnCharacterAgainstTheWall()
    {
        if (IsCharacterBackNotAgainstTheWall())
            DOTween.To(() => transform.forward, x => transform.forward = x, normalToWallPlane, timeToCompleteCharacterOrientation);
    }

    public void SetNormalToWallPlane(Vector3 normalPlane)
    {
        normalToWallPlane = normalPlane;
    }

    private bool IsCharacterBackNotAgainstTheWall()
    {
        return !Mathf.Approximately(Vector3.Dot(transform.forward, normalToWallPlane), 1f);
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

    // TODO: refactor this OnMove repeated code from CharacterRunningState
    void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            Vector3 inputBuffer = inputValue.Get<Vector2>();

            // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
            if (inputBuffer != Vector3.zero)
            {
                if (inputBuffer.y != 0f)
                    inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

                movementDirection = inputBuffer;
            }
            else
            {
                movementDirection = Vector3.zero;
            }
        }
    }

    private void OnCrouch(InputValue inputValue)
    {
        float inputBuffer = inputValue.Get<float>();

        if (!Mathf.Approximately(inputBuffer, 0f))
        {
            crouchingState.enabled = true;
            removeCharacterFromWall.Invoke();
            this.enabled = false;
        }
    }
}
