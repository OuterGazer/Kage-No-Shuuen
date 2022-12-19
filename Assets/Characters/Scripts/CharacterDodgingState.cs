using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterDodgingState : CharacterMovementBase
{
    [SerializeField] float impulseThreshold = 3f;

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

        charController.Move(currentSpeed * Time.deltaTime * dodgeFacingDirection);

        EaseOutCurrentSpeed();
    }

    private void EaseOutCurrentSpeed()
    {
        if (currentSpeed >= impulseThreshold)
            currentSpeed = Mathf.Lerp(speed, 0f, EaseOutQuad(1 - (currentSpeed * 0.999f) / speed) * Time.deltaTime);
        else
            currentSpeed = Mathf.Lerp(speed, 0f, t -= 0.05f * Time.deltaTime);

        Debug.Log(currentSpeed);
    }

    private float t;
    private float EaseOutQuad(float x) 
    {
        return t = 1 - (1 - x) * (1 - x);
    }

public void SetDodgeFacingDirection(Vector3 facingDirection)
    {
        dodgeFacingDirection = facingDirection;
    }

    public void ExitDodgingState()
    {
        idleState.enabled = true;
        this.enabled = false;

        OrientateCharacterForward();
    }
}
