using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterIdleState))]
public class CharacterDodgingState : CharacterMovementBase
{
    [Header("Exit States")]
    [SerializeField] CharacterIdleState idleState;

    [HideInInspector] public UnityEvent makeCharaterDodge;

    private Vector3 dodgeFacingDirection;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        makeCharaterDodge.Invoke();
    }

    private void Update()
    {
        transform.forward = dodgeFacingDirection;
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
