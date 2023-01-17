using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEngine : MonoBehaviour
{
    // TODO: I must implement all exit state conditions here, for example so I can't throw hook while blocking, or change to running or crouching while rolling

    private CharacterStateBase[] allStates;
    private CharacterStateBase currentMovementState;
    private CharacterStateBase currentCombatState;

    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToIdle;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToRunning;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToCrouching;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnWall;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnHook;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnAir;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToBlocking;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToDodging;

    


    private void Awake()
    {
        allStates = GetComponents<CharacterStateBase>();
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onMovementStateChange.AddListener(UpdateCurrentMovementState);
            allStates[i].onCombatStateEnablingOrDisabling.AddListener(UpdateCurrentCombatState);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onMovementStateChange.RemoveListener(UpdateCurrentMovementState);
            allStates[i].onCombatStateEnablingOrDisabling.RemoveListener(UpdateCurrentCombatState);
        }

        currentMovementState = null;
        currentCombatState = null;
    }

    // Event called OnEnbale() of movement states
    private void UpdateCurrentMovementState(CharacterStateBase stateCharacterJustTransitionedTo)
    {
        currentMovementState = stateCharacterJustTransitionedTo;
    }

    // Event called OnEnable() and OnDisable() of combat states
    private void UpdateCurrentCombatState(CharacterStateBase stateCharacterJustTransitionedTo)
    {
        currentCombatState = stateCharacterJustTransitionedTo;
    }

    private bool IsCurrentStateAllowedToTransitionToDesiredState(CharacterStateBase[] allowedCurrentStates)
    {
        foreach (CharacterStateBase state in allowedCurrentStates)
        {
            if (state.Equals(currentMovementState) && currentCombatState == null)
            {
                return true;
            }
        }
        return false;
    }

    private void TransitionToDesiredState(Type state)
    {
        CharacterStateBase stateToTransition = allStates.First(x => x.GetType() == state);
        stateToTransition.enabled = true;
    }

    public void OnDodge()
    {
        if (IsCurrentStateAllowedToTransitionToDesiredState(statesAllowedToTransitionToDodging))
        {
            TransitionToDesiredState(typeof(CharacterDodgingState));
        }
        
    }

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (temp > 0f)
        {
            if (IsCurrentStateAllowedToTransitionToDesiredState(statesAllowedToTransitionToBlocking))
            {
                TransitionToDesiredState(typeof(CharacterBlockingState));
            }
        }
    }
}
