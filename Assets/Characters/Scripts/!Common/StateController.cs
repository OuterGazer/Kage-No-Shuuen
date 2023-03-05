using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateController : MonoBehaviour
{
    // TODO: Refactor the code relating to keep moving after falling or dodging somewhere else?

    private CharacterStateBase[] allStates;
    private CharacterStateBase currentState;

    private CharacterOnHookState onHookState;
    private StealthChecker stealthChecker;

    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToIdle;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToRunning;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToCrouching;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnWall;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnHook;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToOnAir;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToBlocking;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToDodging;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToStealthKill;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToCloseCombat;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToAiming;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToThrowing;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToGettingHit;
    [SerializeField] private CharacterStateBase[] statesAllowedToTransitionToDying;

    private PlayerInput playerInput;
    private InputAction move;

    private CharacterController charController;
    private WeaponController weaponController;
    private Weapon currentWeapon;
    private DamageableWithLife damageable;

    private bool isCrouching = false;
    private bool isAiming = false;


    private void Awake()
    {
        allStates = GetComponents<CharacterStateBase>();
        weaponController = GetComponent<WeaponController>();
        charController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        damageable = GetComponent<DamageableWithLife>();
        stealthChecker = GetComponent<StealthChecker>();
    }

    private void OnEnable()
    {
        GetReferenceToOnHookState();
        onHookState.CanNotFindHookTarget.AddListener(TransitionToIdle);
        weaponController.onWeaponChange.AddListener(SetCurrentWeapon);
        damageable.OnGettingHit.AddListener(TransitionToHit);
        damageable.OnDying.AddListener(TransitionToDeath);

        move = playerInput.actions["Move"];

        foreach (CharacterStateBase state in allStates)
        {
            state.enabled = false;
        }
        currentState = allStates.First(x => x.GetType() == typeof(CharacterIdleState));
        currentState.enabled = true;
    }

    private void GetReferenceToOnHookState()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            CharacterStateBase tempState = allStates[i];
            if (tempState.GetType() == typeof(CharacterOnHookState))
            {
                onHookState = tempState as CharacterOnHookState;
            }
        }
    }

    private void OnDestroy()
    {
        onHookState.CanNotFindHookTarget.RemoveListener(TransitionToIdle);
        weaponController.onWeaponChange.RemoveListener(SetCurrentWeapon);

        currentState = null;
    }

    private void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void Update()
    {
        UpdateIsGroundedState();
    }

    private void UpdateIsGroundedState()
    {
        if (IsCharacterFallingDown())
        {
            TransitionToOnAir();
            move.Disable();
        }
        else if (HasCharacterJustLandedOnGround())
        {
            TransitionToIdle();
            move.Enable();
        }
    }

    private bool IsCharacterFallingDown()
    {
        return (!charController.isGrounded) && (charController.velocity.y < 0f);
    }

    private bool HasCharacterJustLandedOnGround()
    {
        return (charController.isGrounded) && (currentState.GetType() == typeof(CharacterOnAirState));
    }

    private void TransitionToOnAir()
    {
        ManageStateTransition(statesAllowedToTransitionToOnAir, typeof(CharacterOnAirState));
    }

    private void TransitionToIdle()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
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
        currentState.enabled = false;        
        currentState = stateToTransition;
        stateToTransition.enabled = true;
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
            if (currentState.GetType() == typeof(CharacterOnAirState) ||
                currentState.GetType() == typeof(CharacterDodgingState) ||
                currentState.GetType() == typeof(CharacterOnHookState)) { return; } // TODO: Has to do with moving right after landing, try to decouple this!!
            
            if (currentState.GetType() == typeof(CharacterBlockingState) ||
                currentState.GetType() == typeof(CharacterShootingState)||
                currentState.GetType() == typeof(CharacterStealthKillState)) { return; } // Keep blocking/aiming if we were so already

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
            isCrouching = false;

            if (IsCharacterStill())
            {
                if (IsCharacterInAnUnfinishedAnimation()) { return; }
                
                ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
            }
            else
            {
                ManageStateTransition(statesAllowedToTransitionToRunning, typeof(CharacterRunningState));
            }
        }
    }

    private bool IsCharacterInAnUnfinishedAnimation()
    {
        return currentState.GetType() == typeof(CharacterOnHookState) ||
               currentState.GetType() == typeof(CharacterOnAirState) ||
               currentState.GetType() == typeof(CharacterStealthKillState);
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
        if (currentState.GetType() == typeof(CharacterDodgingState))
        { move.Disable(); }
    }

    // Called from dodging animation event
    public void SetInvincibility(int isInvincible)
    {
        charController.detectCollisions = (isInvincible == 1) ? false : true; // Invincible against trigger weapons with rigibody
        gameObject.layer = (isInvincible == 1) ? 0 : 3;//LayerMask.GetMask("Player"); // Invincible against weapons with raycast

        // TODO: investigate weird bug. If I do GetMask() instead of assigning the number directly, raycasts don't recognize the layer properly
        // (for example hook doesn't work because it thinks the character controller is an obstacle)
    }

    // Called from an animation event in the rolling animation
    public void ExitDodgingState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
        move.Enable();
    }

    public void OnBlock(InputValue inputValue)
    {
        if (currentWeapon.CanBlock)
        {
            float temp = inputValue.Get<float>();

            if (!Mathf.Approximately(temp, 0f))
            {
                CharacterBlockingState blockingstate = GetComponent<CharacterBlockingState>(); // is done once, not that big of a deal
                blockingstate.SetIs2HWeapon(currentWeapon.Is2H);
                
                ManageStateTransition(statesAllowedToTransitionToBlocking, typeof(CharacterBlockingState));
            }
            else
            {
                //currentState?.ExitState();
                ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
            }
        }
    }

    public void OnSlash() 
    {
        if(stealthChecker.CanPerformStealthKill)
        {
            CharacterStealthKillState.SetCurrentWeapon(currentWeapon);
            ManageStateTransition(statesAllowedToTransitionToStealthKill, typeof(CharacterStealthKillState));
        }
        else
        {
            ManageStateTransition(statesAllowedToTransitionToCloseCombat, typeof(CharacterCloseCombatState));
        }
    }

    public void ExitStealthKillState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
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

    private void TransitionToHit()
    {
        ManageStateTransition(statesAllowedToTransitionToGettingHit, typeof(CharacterHitState));
        playerInput.enabled = false;
    }

    public void ExitHitState()
    {
        ManageStateTransition(statesAllowedToTransitionToIdle, typeof(CharacterIdleState));
        playerInput.enabled = true;
    }

    private void TransitionToDeath()
    {
        ManageStateTransition(statesAllowedToTransitionToDying, typeof(CharacterDeadState));
        playerInput.enabled = false;
    }
}
