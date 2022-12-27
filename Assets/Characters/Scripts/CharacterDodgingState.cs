using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterDodgingState : CharacterMovementBase
{
    [SerializeField] float speedDeceleration = 0.05f;

    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [HideInInspector] public UnityEvent makeCharaterDodge;

    private Vector3 dodgeFacingDirection;
    private float currentSpeed;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        makeCharaterDodge.Invoke();
        currentSpeed = speed;
    }

    private void Update()
    {
        transform.forward = dodgeFacingDirection;

        Vector3 movementOnPlane = currentSpeed * Time.deltaTime * dodgeFacingDirection;
        Vector3 verticalMovement = new Vector3(0f, Physics.gravity.y * Time.deltaTime, 0f);

        charController.Move(movementOnPlane + verticalMovement);
        EaseOutCurrentSpeed();
    }

    private void EaseOutCurrentSpeed()
    {
        currentSpeed -= speedDeceleration * Time.deltaTime;
    }

public void SetDodgeFacingDirection(Vector3 facingDirection)
    {
        dodgeFacingDirection = facingDirection;
    }

    // Called from an animation event in the rolling animation
    public void ExitDodgingState()
    {
        OrientateCharacterForward();

        idleState.enabled = true;
        this.enabled = false;
    }
}
