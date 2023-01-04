using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterCrouchingState), typeof(CharacterOnWallState))]
[RequireComponent(typeof(CharacterOnHookState), typeof(CharacterOnAirState))]
public class CharacterAnimator : MonoBehaviour
{
    [HideInInspector] public UnityEvent hookHasArrivedAtTarget;

    private Animator animator;

    private CharacterCrouchingState crouchingState;
    private CharacterOnWallState onWallState;
    private CharacterOnHookState onHookState;
    private CharacterOnAirState onAirState;
    private CharacterDodgingState dodgingState;
    private CharacterBlockingState blockingState;
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

    Vector3 oldPosition;

    private bool isAnimationCorrectedWhenOnWall = false;

    private void Awake()
    {
        GetNecessaryComponents();
        AddNecessaryListeners();
    }

    private void GetNecessaryComponents()
    {
        animator = GetComponentInChildren<Animator>();

        runningSpeed = GetComponent<CharacterRunningState>().Speed;

        crouchingState = GetComponent<CharacterCrouchingState>();
        onWallState = GetComponent<CharacterOnWallState>();
        onHookState = GetComponent<CharacterOnHookState>();
        onAirState = GetComponent<CharacterOnAirState>();
        dodgingState = GetComponent<CharacterDodgingState>();
        blockingState = GetComponent<CharacterBlockingState>();
        weaponController = GetComponent<WeaponController>();
    }

    private void AddNecessaryListeners()
    {
        crouchingState.AttachCharacterToWall.AddListener(AttachCharacterToWall);
        onWallState.RemoveCharacterFromWall.AddListener(RemoveCharacterFromWall);
        onWallState.CorrectCharacterAnimationWhenCameraIsNearWall.AddListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.AddListener(HaveCharacterThrowHook);
        onHookState.ChangeToHangingAnimation.AddListener(TransitionToOrFromHooked);
        onAirState.ChangeToLandingAnimation.AddListener(TriggerLandingAnimation);
        onAirState.IsCharacterTouchingGround.AddListener(TransitionToOrFromAir);
        dodgingState.MakeCharacterDodge.AddListener(PlayDodgeAnimation);
        blockingState.UpdateBlockingStatus.AddListener(UpdateBlocking);
        weaponController.onWeaponChange.AddListener(PlayChangeWeaponAnimation);
        weaponController.onSlash.AddListener(PlaySlashAnimation);
        weaponController.onHeavySlash.AddListener(PlayHeavySlashAnimation); 
    }    

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void RemoveListeners()
    {
        crouchingState.AttachCharacterToWall.RemoveListener(AttachCharacterToWall);
        onWallState.RemoveCharacterFromWall.RemoveListener(RemoveCharacterFromWall);
        onWallState.CorrectCharacterAnimationWhenCameraIsNearWall.RemoveListener(SetCorrectAnimationWhenCharacterIsOnWall);
        onHookState.throwHook.RemoveListener(HaveCharacterThrowHook);
        onHookState.ChangeToHangingAnimation.RemoveListener(TransitionToOrFromHooked);
        onAirState.ChangeToLandingAnimation.RemoveListener(TriggerLandingAnimation);
        onAirState.IsCharacterTouchingGround.RemoveListener(TransitionToOrFromAir);
        dodgingState.MakeCharacterDodge.RemoveListener(PlayDodgeAnimation);
        blockingState.UpdateBlockingStatus.AddListener(UpdateBlocking);
        weaponController.onWeaponChange.AddListener(PlayChangeWeaponAnimation);
        weaponController.onSlash.RemoveListener(PlaySlashAnimation);
        weaponController.onHeavySlash.RemoveListener(PlayHeavySlashAnimation);
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
    }

    public void ApplyAnimatorController(Weapon weapon)
    {
        if (weapon.animatorOverride)
            animator.runtimeAnimatorController = weapon.animatorOverride;
        else if(!weapon.animatorOverride)
            animator.runtimeAnimatorController = standardAnimatorController;

        CorrectIsGroundedBug(); // TODO: look for the actual reason this happens and fix it
    }

    private void CorrectIsGroundedBug()
    {
        if (!animator.GetBool(isGroundedHash))
        {
            animator.SetTrigger(landingHash);
            animator.SetBool(isGroundedHash, true);
        }
            
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
    
    void OnMove(InputValue inputValue)
    {
        Vector3 inputBuffer = inputValue.Get<Vector2>();

        // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
        if (inputBuffer != Vector3.zero)
        {
            if (inputBuffer.y != 0f)
                inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);

            movementDirection = inputBuffer;
        }
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

    public void PlayChangeWeaponAnimation()
    {
        animator.SetTrigger(changeWeaponHash);
    }

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
}
