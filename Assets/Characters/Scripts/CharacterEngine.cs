using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEngine : MonoBehaviour
{
    // TODO: I must implement all exit state conditions here, for example so I can't throw hook while blocking, or change to running or crouching while rolling

    [SerializeField] private CharacterMovementBase[] movementStates = { };
    [SerializeField] private CharacterBlockingState blockingState;
    [SerializeField] private CharacterDodgingState dodgingState;

    [SerializeField] private CharacterMovementBase[] allowedStatesForBlocking = { };
    [SerializeField] private CharacterMovementBase[] allowedStatesForDodging = { };

    private CharacterMovementBase currentMovementState;
    private CharacterMovementBase currentCombatState;


    private void Awake()
    {
        for (int i = 0; i < movementStates.Length; i++)
        {
            movementStates[i].onMovementStateChange.AddListener(UpdateCurrentMovementState);
        }

        blockingState.onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
        dodgingState.onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < movementStates.Length; i++)
        {
            movementStates[i].onMovementStateChange.RemoveListener(UpdateCurrentMovementState);
        }

        blockingState.onCombatStateEnablingOrDisabling.RemoveListener(UpdateCurrentCombatState);
        dodgingState.onCombatStateEnablingOrDisabling.RemoveListener(UpdateCurrentCombatState);

        currentMovementState = null;
        currentCombatState = null;
    }

    private void UpdateCurrentMovementState(CharacterMovementBase enablingState)
    {
        currentMovementState = enablingState;
    }

    private void UpdateCurrentCombatState(CharacterMovementBase enablingState)
    {
        currentCombatState = enablingState;
    }

    public void OnDodge()
    {
        EnableCombatStateIfInAllowedMovementState(allowedStatesForDodging, dodgingState);
    }

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (temp > 0f)
        {
            EnableCombatStateIfInAllowedMovementState(allowedStatesForBlocking, blockingState);
        }
    }

    private void EnableCombatStateIfInAllowedMovementState(CharacterMovementBase[] allowedStates, CharacterMovementBase combatStateToEnable)
    {
        foreach (CharacterMovementBase state in allowedStates)
        {
            if (state.Equals(currentMovementState) && currentCombatState == null)
                combatStateToEnable.enabled = true;
        }
    }
}
