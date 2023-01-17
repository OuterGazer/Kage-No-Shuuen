using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEngine : MonoBehaviour
{
    // TODO: I must implement all exit state conditions here, for example so I can't throw hook while blocking, or change to running or crouching while rolling

    [SerializeField] private CharacterStateBase[] allStates = { }; // Serialized for testing purposes
    [SerializeField] private CharacterBlockingState blockingState;
    [SerializeField] private CharacterDodgingState dodgingState;

    [SerializeField] private CharacterStateBase[] allowedStatesForBlocking = { };
    [SerializeField] private CharacterStateBase[] allowedStatesForDodging = { };

    private CharacterStateBase currentMovementState;
    private CharacterStateBase currentCombatState;


    private void Awake()
    {
        allStates = GetComponents<CharacterStateBase>();
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onMovementStateChange.AddListener(UpdateCurrentMovementState);
            allStates[i].onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
        }

        //blockingState.onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
        //dodgingState.onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onMovementStateChange.RemoveListener(UpdateCurrentMovementState);
        }

        blockingState.onCombatStateEnablingOrDisabling.RemoveListener(UpdateCurrentCombatState);
        dodgingState.onCombatStateEnablingOrDisabling.RemoveListener(UpdateCurrentCombatState);

        currentMovementState = null;
        currentCombatState = null;
    }

    private void UpdateCurrentMovementState(CharacterStateBase enablingState)
    {
        currentMovementState = enablingState;
    }

    private void UpdateCurrentCombatState(CharacterStateBase enablingState)
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

    private void EnableCombatStateIfInAllowedMovementState(CharacterStateBase[] allowedStates, CharacterStateBase combatStateToEnable)
    {
        foreach (CharacterStateBase state in allowedStates)
        {
            if (state.Equals(currentMovementState) && currentCombatState == null)
                combatStateToEnable.enabled = true;
        }
    }
}
