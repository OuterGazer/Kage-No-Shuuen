using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
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

    private bool isCrouching = false;


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

    // Event called OnEnable() of movement states
    private void UpdateCurrentMovementState(CharacterStateBase stateCharacterJustTransitionedTo)
    {
        if (currentMovementState) { currentMovementState.enabled = false; }
            
        currentMovementState = stateCharacterJustTransitionedTo;
    }

    // Event called OnEnable() and OnDisable() of combat states
    private void UpdateCurrentCombatState(CharacterStateBase stateCharacterJustTransitionedTo)
    {
        currentCombatState = stateCharacterJustTransitionedTo;
    }

    private void ManageStateTransition(CharacterStateBase[] allowedCurrentStates, Type state)
    {
        if (IsCurrentStateAllowedToTransitionToDesiredState(allowedCurrentStates))
        {
            TransitionToDesiredState(state);
        }
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

    public void OnMove(InputValue inputValue)
    {
        if (inputValue.Get<Vector2>() != Vector2.zero)
        {
            if (currentMovementState.GetType() == typeof(CharacterCrouchingState)) { return; } // Keep crouching if we were alredy crouching

            if(!isCrouching)
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            else
                ManageStateTransition(statesAllowedToTransitionToCrouching, typeof(CharacterCrouchingState)); // We were in idle
        }
        else
        {
            ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
        }
    }

    private void OnCrouch(InputValue inputValue)
    {
        if (IsCrouchButtonPressed(inputValue))
        {
            ManageStateTransition(statesAllowedToTransitionToCrouching, typeof(CharacterCrouchingState));
            isCrouching = true;
        }
        else if (IsCrouchButtonReleased(inputValue))
        {
            if (IsCharacterStill())
            {
                ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
            }
            else
            {
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            }
            isCrouching = false;
        }
    }

    private bool IsCharacterStill()
    {
        return CharacterStateBase.MovementDirection == Vector3.zero;
    }

    private static bool IsCrouchButtonPressed(InputValue inputValue)
    {
        return !Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    private static bool IsCrouchButtonReleased(InputValue inputValue)
    {
        return Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Cover"))
        {
            CharacterOnWallState.SetNormalToWallPlane(hit.normal);
            ManageStateTransition(statesAllowedToTransitionToOnWall, typeof(CharacterOnWallState));
        }
    }

    public void OnDodge()
    {
        ManageStateTransition(statesAllowedToTransitionToDodging, typeof(CharacterDodgingState));
    }

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (temp > 0f)
        {
            ManageStateTransition(statesAllowedToTransitionToBlocking, typeof(CharacterBlockingState));
        }
    }
}
