using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEngine : MonoBehaviour
{
    // TODO: Have moving after dodging or OnAir logic here somehow,
    //       as of now Crouching, Running, OnHook, OnAir and Dodging are couple directly to Idle because of this.

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
            allStates[i].onCombatStateEnteringOrExiting.AddListener(UpdateCurrentCombatState);
            allStates[i].onNeedingToTransitionToIdle.AddListener(TransitionToIdle);
            allStates[i].onBeingOnAir.AddListener(TransitionToOnAir);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onMovementStateChange.RemoveListener(UpdateCurrentMovementState);
            allStates[i].onCombatStateEnteringOrExiting.RemoveListener(UpdateCurrentCombatState);
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

    // Event called on entering or exiting combat states
    private void UpdateCurrentCombatState(CharacterStateBase stateCharacterJustTransitionedTo)
    {
        if (stateCharacterJustTransitionedTo == null)
            { currentCombatState.enabled = false; }
        
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

    // Specific method called from event to transition to idle when failing a hook throw or coming from OnAir state
    private void TransitionToIdle()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
    }

    // Specific method called from event to transition to OnAir when after hook throwing or falling off a ledge
    private void TransitionToOnAir()
    {
        ManageStateTransition(statesAllowedToTransitionToOnAir, typeof(CharacterOnAirState));
    }
    
    public void OnMove(InputValue inputValue)
    {
        if (inputValue.Get<Vector2>() != Vector2.zero)
        {
            if (currentMovementState.GetType() == typeof(CharacterCrouchingState)) { return; } // Keep crouching if we were alredy crouching

            if(!isCrouching)
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            else
                ManageStateTransition(statesAllowedToTransitionToCrouching, typeof(CharacterCrouchingState)); // Enable transitioning from idle to crouching
        }
        else
        {
            if (currentMovementState.GetType() == typeof(CharacterOnAirState)) { return; } // Has to do with moving right after landing, try to decouple this!!

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
                if (IsCharacterOnHookOrOnAir()) { return; }
                
                ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
            }
            else
            {
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            }
            isCrouching = false;
        }
    }

    private bool IsCharacterOnHookOrOnAir()
    {
        return currentMovementState.GetType() == typeof(CharacterOnHookState) ||
               currentMovementState.GetType() == typeof(CharacterOnAirState);
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

    public void OnHookThrow()
    {
        ManageStateTransition(statesAllowedToTransitionToOnHook, typeof(CharacterOnHookState));
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
