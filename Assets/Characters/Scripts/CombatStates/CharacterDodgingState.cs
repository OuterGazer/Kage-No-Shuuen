using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDodgingState : CharacterStateBase
{
    [SerializeField] float speedDeceleration = 0.05f;

    [HideInInspector] public UnityEvent MakeCharacterDodge;

    private Vector3 dodgeFacingDirection;
    private float currentSpeed;

    private void OnEnable()
    {
        SetDodgeFacingDirection(charController.velocity.normalized);
        MakeCharacterDodge.Invoke();
        currentSpeed = speed;
        ChangeCameraType();
    }

    private void OnDisable()
    {
        OrientateCharacterForward();
        ChangeCameraType();
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
}
