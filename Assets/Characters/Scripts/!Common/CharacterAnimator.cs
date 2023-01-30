using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterCrouchingState), typeof(CharacterOnWallState))]
//[RequireComponent(typeof(CharacterOnHookState), typeof(CharacterOnAirState))]
public class CharacterAnimator : MonoBehaviour
{
    [HideInInspector] public UnityEvent hookHasArrivedAtTarget;

    private Animator animator;

    private CharacterStateBase stateBase;
    private CharacterOnWallState onWallState;
    private CharacterOnHookState onHookState;
    private CharacterOnAirState onAirState;
    private CharacterDodgingState dodgingState;
    private CharacterBlockingState blockingState;
    private CharacterCloseCombatState closeCombatState;
    private CharacterShootingState shootingState;
    private CharacterThrowingState throwingState;

    private Vector3 movementDirection;
    private float runningSpeed;

    private WeaponController weaponController;
    private RuntimeAnimatorController standardAnimatorController;

    int movementForwardHash;
    int movementSidewaysHash;
    int onWallHash;
    int throwHookHash;
    int isGroundedHash;
    int isHookedHash;
    int landingHash;
    int dodgingHash;
    int isBlockingHash;
    int changeWeaponHash;
    int slashHash;
    int canChainComboHash;
    int heavySlashHash;
    int isAimingHash;
    int shootHash;
    int throwingHash;

    Vector3 oldPosition;

    private bool isAnimationCorrectedWhenOnWall = false;

    private void Awake()
    {
        GetNecessaryComponents();
        if(CompareTag("Player"))
            AddNecessaryListeners();
    }

    private void GetNecessaryComponents()
    {
        animator = GetComponentInChildren<Animator>();

        runningSpeed = GetComponent<CharacterRunningState>().Speed;

        stateBase = GetComponent<CharacterStateBase>();
        onWallState = GetComponent<CharacterOnWallState>();
        onHookState = GetComponent<CharacterOnHookState>();
        onAirState = GetComponent<CharacterOnAirState>();
        dodgingState = GetComponent<CharacterDodgingState>();
        blockingState = GetComponent<CharacterBlockingState>();
        weaponController = GetComponent<WeaponController>();
        closeCombatState = GetComponent<CharacterCloseCombatState>();
        shootingState= GetComponent<CharacterShootingState>();
        throwingState= GetComponent<CharacterThrowingState>();
    }

    private void AddNecessaryListeners()
    {
        stateBase.onMovementSpeedChange.AddListener(SetMovementDirection);
        onWallState.AttachCharacterToWall.AddListener(AttachCharacterToWall);
        onWallState.RemoveCharacterFromWall.AddListener(RemoveCharacterFromWall);
        onWallState.CorrectCharacterAnimationWhenCameraIsNearWall.AddListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.AddListener(HaveCharacterThrowHook);
        onHookState.ChangeToHangingAnimation.AddListener(TransitionToOrFromHooked);
        onAirState.ChangeToLandingAnimation.AddListener(TriggerLandingAnimation);
        onAirState.IsCharacterTouchingGround.AddListener(TransitionToOrFromAir);
        dodgingState.MakeCharacterDodge.AddListener(PlayDodgeAnimation);
        blockingState.UpdateBlockingStatus.AddListener(UpdateBlocking);
        weaponController.onWeaponChange.AddListener(PlayChangeWeaponAnimation);
        closeCombatState.onSlash.AddListener(PlaySlashAnimation);
        closeCombatState.onHeavySlash.AddListener(PlayHeavySlashAnimation);
        shootingState.onAim.AddListener(PlayAimingAnimation);
        shootingState.onShoot.AddListener(PlayShootingAnimation);
        throwingState.onThrowing.AddListener(PlayThrowingAnimation);
    }    

    private void OnDestroy()
    {
        if (CompareTag("Player"))
            RemoveListeners();
    }

    private void RemoveListeners()
    {
        stateBase.onMovementSpeedChange.RemoveListener(SetMovementDirection);
        onWallState.AttachCharacterToWall.RemoveListener(AttachCharacterToWall);
        onWallState.RemoveCharacterFromWall.RemoveListener(RemoveCharacterFromWall);
        onWallState.CorrectCharacterAnimationWhenCameraIsNearWall.RemoveListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.RemoveListener(HaveCharacterThrowHook);
        onHookState.ChangeToHangingAnimation.RemoveListener(TransitionToOrFromHooked);
        onAirState.ChangeToLandingAnimation.RemoveListener(TriggerLandingAnimation);
        onAirState.IsCharacterTouchingGround.RemoveListener(TransitionToOrFromAir);
        dodgingState.MakeCharacterDodge.RemoveListener(PlayDodgeAnimation);
        blockingState.UpdateBlockingStatus.RemoveListener(UpdateBlocking);
        weaponController.onWeaponChange.RemoveListener(PlayChangeWeaponAnimation);
        closeCombatState.onSlash.RemoveListener(PlaySlashAnimation);
        closeCombatState.onHeavySlash.RemoveListener(PlayHeavySlashAnimation);
        shootingState.onAim.RemoveListener(PlayAimingAnimation);
        shootingState.onShoot.RemoveListener(PlayShootingAnimation);
        throwingState.onThrowing.RemoveListener(PlayThrowingAnimation);
    }

    private void Start()
    {
        GenerateHashes();

        standardAnimatorController = animator.runtimeAnimatorController;
        animator.SetBool(isGroundedHash, true);
        oldPosition = transform.position;
    }

    private void GenerateHashes()
    {
        movementForwardHash = Animator.StringToHash("movementForward");
        movementSidewaysHash = Animator.StringToHash("movementSideways");
        onWallHash = Animator.StringToHash("OnWall");
        throwHookHash = Animator.StringToHash("throwHook");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isHookedHash = Animator.StringToHash("isHooked");
        landingHash = Animator.StringToHash("Landing");
        dodgingHash = Animator.StringToHash("Dodging");
        isBlockingHash = Animator.StringToHash("isBlocking");
        changeWeaponHash = Animator.StringToHash("ChangeWeapon");
        slashHash = Animator.StringToHash("Slash");
        canChainComboHash = Animator.StringToHash("canChainCombo");
        heavySlashHash = Animator.StringToHash("HeavySlash");
        isAimingHash = Animator.StringToHash("isAiming");
        shootHash = Animator.StringToHash("Shoot");
        throwingHash = Animator.StringToHash("Throw");
    }

    private void SetMovementDirection(Vector3 movementDirection)
    {
        this.movementDirection = movementDirection;
    }

    public void ApplyAnimatorController(Weapon weapon)
    {
        if (weapon.AnimatorOverride)
            animator.runtimeAnimatorController = weapon.AnimatorOverride;
        else if(!weapon.AnimatorOverride)
            animator.runtimeAnimatorController = standardAnimatorController;
    }

    private void Update()
    {
        UpdateStandingAnimationTransitions();

        oldPosition = transform.position;
    }

    float amountOfForwardMovement;
    float amountOfSidewaysMovement;
    float forwardMovementDirection;
    float sidewaysMovementDirection;
    float currentVelocityForwardNormalized;
    float currentVelocitySidewaysNormalized;
    private void UpdateStandingAnimationTransitions()
    {
        CalculateDistanceMovedLastFrameProAxis();
        CalculateSignOfMovementDirection();
        CalculateMovementSpeedProAxis();

        ApplyAnimationTransitionValues();
    }

    private void CalculateDistanceMovedLastFrameProAxis()
    {
        Vector3 distanceMoved = transform.position - oldPosition;
        amountOfForwardMovement = Vector3.Project(distanceMoved, transform.forward).magnitude;
        amountOfSidewaysMovement = Vector3.Project(distanceMoved, transform.right).magnitude;
    }

    private void CalculateSignOfMovementDirection()
    {
        if (IsPlayerPressingAMovementKey(movementDirection.z))
            forwardMovementDirection = Mathf.Sign(movementDirection.z);

        if (IsPlayerPressingAMovementKey(movementDirection.x))
            sidewaysMovementDirection = Mathf.Sign(movementDirection.x);

        // Releasing a key keeps the last sign, avoiding that the character
        // briefly looks the other way while decelerating when sign is -1.

        if (CompareTag("Soldier"))
        {
            forwardMovementDirection = GetComponent<NavMeshAgent>().velocity.z;
            sidewaysMovementDirection = GetComponent<NavMeshAgent>().velocity.x;
        }
            

    }

    private bool IsPlayerPressingAMovementKey(float movementAxis)
    {
        return !Mathf.Approximately(movementAxis, 0f);
    }

    private void CalculateMovementSpeedProAxis()
    {
        // Blend tree uses normalized values 1 for running speed and 0,5 for crouching speed
        // A change here must take in account a change in the blend tree
        // TODO: look for a way to decouple this
        currentVelocityForwardNormalized = (amountOfForwardMovement / Time.deltaTime) / runningSpeed; 
        currentVelocitySidewaysNormalized = (amountOfSidewaysMovement / Time.deltaTime) / runningSpeed;
    }

    private void ApplyAnimationTransitionValues()
    {
        animator.SetFloat(movementForwardHash, currentVelocityForwardNormalized * forwardMovementDirection);

        if (isAnimationCorrectedWhenOnWall)
        {
            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * (forwardMovementDirection * onWallAnimationCorrectionFactor));
            isAnimationCorrectedWhenOnWall = false;
        }
        else
        {
            animator.SetFloat(movementSidewaysHash, currentVelocitySidewaysNormalized * sidewaysMovementDirection);
        }
    }

    private float onWallAnimationCorrectionFactor = 1f;

    private void SetCorrectAnimationWhenCharacterIsOnWall(float directionSign)
    {
        onWallAnimationCorrectionFactor = directionSign;
        isAnimationCorrectedWhenOnWall = true;
    }

    public void AttachCharacterToWall()
    {
        animator.SetBool(onWallHash, true);
    }

    public void RemoveCharacterFromWall()
    {
        animator.SetBool(onWallHash, false);
    }

    public void HaveCharacterThrowHook()
    {
        animator.SetTrigger(throwHookHash);
    }

    public void TransitionToOrFromHooked(bool isHooked)
    {
        animator.SetBool(isHookedHash, isHooked);
    }

    public void TriggerLandingAnimation()
    {
        animator.SetTrigger(landingHash);
    }

    public void TransitionToOrFromAir(bool isGrounded)
    {
        animator.SetBool(isGroundedHash, isGrounded);
    }

    public void HookHasArrivedAtTarget()
    {
        hookHasArrivedAtTarget.Invoke();
    }

    public void PlayDodgeAnimation()
    {
        animator.SetTrigger(dodgingHash);
    }

    public void UpdateBlocking(bool isBlocking)
    {
        animator.SetBool(isBlockingHash, isBlocking);
    }

    // TODO: have ApplyAnimatorController() here, that would decouple better WeaponController and this script
    public void PlayChangeWeaponAnimation(Weapon weapon)
    {
        if (!weapon)
        { animator.SetTrigger(changeWeaponHash); }
    }

    //TODO: Have character correctly grab 2-Hand weapons with both hands
    public void PlaySlashAnimation()
    {
        animator.SetTrigger(slashHash);
    }

    // Called through animation events
    // Needs 2 in same animation, 1 == true, 0 == false
    public void SetCanChainCombo(int canChain)
    {
        if(canChain == 1)
            { animator.SetBool(canChainComboHash, true); }
        else if(canChain == 0)
            { animator.SetBool(canChainComboHash, false); }

    }

    public void PlayHeavySlashAnimation()
    {
        animator.SetTrigger(heavySlashHash);
    }

    
    public void PlayAimingAnimation(bool isAiming)
    {
        animator.SetBool(isAimingHash, isAiming);
    }

    public void PlayShootingAnimation()
    {
        animator.SetTrigger(shootHash);
    }

    public void PlayThrowingAnimation()
    {
        // TODO: desde WeaponController pasar un parámetro para aquí usar animator.speed y establecer la velocidad de animación.
        animator.SetTrigger(throwingHash);
    }
}
