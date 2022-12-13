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
    private LayerMask coverMask;

    private float charControllerHorizontalBound;

    private bool isCharacterAtCoverEdge = false;

    private void Awake()
    {
        this.enabled = false;
    }

    private void Start()
    {
        coverMask = LayerMask.GetMask("Cover");
        charControllerHorizontalBound = GetComponent<CharacterController>().bounds.extents.x;
    }

    private void OnEnable()
    {
        isCharacterAtCoverEdge = false;
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, transform.forward);
    }

    private void FixedUpdate()
    {
        if (HasCoverEdgeBeenReached())
            { movementDirection = Vector3.zero; }
        else if (isCharacterAtCoverEdge)
            { isCharacterAtCoverEdge = false; }
    }

    [SerializeField] float timeToCompleteCharacterOrientation = 0.25f;
    public void turnCharacterToWall(Vector3 normalPlane)
    {
        DOTween.To(() => transform.forward, x => transform.forward = x, normalPlane, timeToCompleteCharacterOrientation);
    }

    [SerializeField] float stoppingDistanceToCoverEdge = 3.75f;
    private bool HasCoverEdgeBeenReached()
    {
        float characterMovementDirectionOnWall = Mathf.Sign(Vector3.Dot(transform.right, currentHorizontalMovement));

        Vector3 raycastOriginPoint = transform.position + charControllerHorizontalBound * characterMovementDirectionOnWall *
                                                          stoppingDistanceToCoverEdge * transform.right;

        return isCharacterAtCoverEdge = !Physics.Raycast(raycastOriginPoint, -transform.forward, 0.5f, coverMask);
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

            this.enabled = false;
        }
    }
}
