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
    //       Due to this there is some spaghetti in idle

    private CharacterStateBase[] allStates;
    private CharacterStateBase currentState;

    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToIdle;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToRunning;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToCrouching;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnWall;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnHook;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnAir;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToBlocking;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToDodging;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToCloseCombat;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToAiming;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToThrowing;

    private WeaponController weaponController;
    private Weapon currentWeapon;

    private bool isCrouching = false;
    private bool isAiming = false;


    private void Awake()
    {
        allStates = GetComponents<CharacterStateBase>();
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onNeedingToTransitionToIdle.AddListener(TransitionToIdle);
            allStates[i].onBeingOnAir.AddListener(TransitionToOnAir);
        }

        weaponController = GetComponent<WeaponController>();
        weaponController.onWeaponChange.AddListener(SetCurrentWeapon);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].onNeedingToTransitionToIdle.RemoveListener(TransitionToIdle);
            allStates[i].onBeingOnAir.RemoveListener(TransitionToOnAir);
        }

        weaponController.onWeaponChange.RemoveListener(SetCurrentWeapon);

        currentState = null;
    }

    private void OnEnable()
    {
        currentState = allStates.First(x => x.GetType() == typeof(CharacterIdleState));
    }

    private void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
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
            if (state.Equals(currentState))
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
        
        currentState.enabled = false;
        currentState = stateToTransition;
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
            if (currentState.GetType() == typeof(CharacterCrouchingState)) { return; } // Keep crouching if we were alredy crouching

            if(!isCrouching)
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            else
                ManageStateTransition(statesAllowedToTransitionToCrouching, typeof(CharacterCrouchingState)); // Enable transitioning from idle to crouching
        }
        else
        {
            if (currentState.GetType() == typeof(CharacterOnAirState)) { return; } // Has to do with moving right after landing, try to decouple this!!
            if (currentState.GetType() == typeof(CharacterBlockingState) ||
                currentState.GetType() == typeof(CharacterShootingState)) { return; } // Keep blocking/aiming if we were so already

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
        return currentState.GetType() == typeof(CharacterOnHookState) ||
               currentState.GetType() == typeof(CharacterOnAirState);
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

    // Called from an animation event in the rolling animation
    public void ExitDodgingState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
    }

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (!Mathf.Approximately(temp, 0f))
        {
            ManageStateTransition(statesAllowedToTransitionToBlocking, typeof(CharacterBlockingState));
        }
        else
        {
            //currentState?.ExitState();
            ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
        }
    }

    public void OnSlash() 
    {
        ManageStateTransition(statesAllowedToTransitionToCloseCombat, typeof(CharacterCloseCombatState));
    }
    public void OnHeavySlash() 
    {
        ManageStateTransition(statesAllowedToTransitionToCloseCombat, typeof(CharacterCloseCombatState));
    }

    // Called from animation event in all attacking animations
    public void ExitCloseCombatState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
    }

    public void OnAim()
    {
        if (currentWeapon?.AimingRig)
        {
            if (!isAiming)
            {
                CharacterShootingState.SetCurrentWeapon(currentWeapon);
                ManageStateTransition(statesAllowedToTransitionToAiming, typeof(CharacterShootingState));
                isAiming = true;
            }
            else
            {
                //currentState?.ExitState();
                ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
                isAiming = false;
            }
        }
    }

    private void OnWeaponThrow() 
    {
        if (currentWeapon?.ThrowingWeaponBase)
        {
            CharacterThrowingState.SetCurrentWeapon(currentWeapon);
            ManageStateTransition(statesAllowedToTransitionToThrowing, typeof(CharacterThrowingState));
        }
    }

    // Called from animation event of all throwing animations
    public void ExitThrowingState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
    }
}
