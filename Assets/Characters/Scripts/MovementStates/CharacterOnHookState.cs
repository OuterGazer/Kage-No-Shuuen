using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterOnAirState), typeof(CharacterIdleState))]
public class CharacterOnHookState : CharacterStateBase
{
    // TODO: pensar en si quiero que el jugador pueda lanzar el gancho estando OnWall
    // TODO: arreglar bug raro donde si aprieto el boton de agacharse durante la animación de lanzar pasan cosas raras

    [SerializeField] float hookThrowRadius = 10f;
    [SerializeField] float hookReachThreshold = 4f;
    private Transform hookTarget;


    [Header("Exit States")]
    [SerializeField] CharacterOnAirState onAirState;
    [SerializeField] CharacterIdleState idleState;

    [Header("Rig Characteristics")]
    [SerializeField] float rigAlignmentToHookTargetAcceleration = 0.01f;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;
    

    [HideInInspector] public UnityEvent throwHook;
    [HideInInspector] public UnityEvent<bool> ChangeToHangingAnimation;
    private CharacterAnimator characterAnimator;
    private Rig spineToFingerRig;
    private LayerMask hookTargetMask;
    private LayerMask obstaclesMask = ~(1 << 3);

    private Vector3 hangingDirection;

    private float currentOnHookSpeed;

    private bool isHookThrown = false;

    private void Awake()
    {
        this.enabled = false;

        characterAnimator = GetComponent<CharacterAnimator>();
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        spineToFingerRig.weight = 0f;
        hookTargetMask = LayerMask.GetMask("HookTarget");
    }

    private void OnDestroy()
    {
        characterAnimator.hookHasArrivedAtTarget.RemoveListener(MoveCharacterToHookTarget);
    }

    private void OnEnable()
    {
        onMovementStateChange.Invoke(this);

        ManageHookThrowing();
    }

    private void OnDisable()
    {
        hookTarget = null;
        SetTargetToRigChain();
    }

    private void ManageHookThrowing()
    {
        CheckIfThereIsHookTargetInRange();        
    }

    private void CheckIfThereIsHookTargetInRange()
    {
        Collider[] hookTargets = Physics.OverlapSphere(transform.position, hookThrowRadius, hookTargetMask);

        if (hookTargets.Length == 0)
        {
            StartCoroutine(ExitToIdle());
        }
        else
        {
            hookTarget = hookTargets[0].transform;
            CheckIfObstaclesBetweenCharacterAndTarget();
        }
    }

    private void CheckIfObstaclesBetweenCharacterAndTarget()
    {
        Vector3 tempHookDir = (hookTarget.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, tempHookDir, hookThrowRadius + 1f, obstaclesMask);

        if (!hits[0].collider.CompareTag("HookTarget"))
        {
            StartCoroutine(ExitToIdle());
        }
        else
        {
            idleState.move.Disable();
            PerformHookThrowing();
        }
    }

    private void PerformHookThrowing()
    {
        SetTargetToRigChain();
        ThrowHook();
    }

    private IEnumerator ExitToIdle()
    {
        this.enabled = false;
        yield return new WaitForEndOfFrame();
        idleState.enabled = true;        
    }

    private void SetTargetToRigChain()
    {
        foreach (ChainIKConstraint item in shoulderToFingerConstraints)
        {
            item.data.target = hookTarget;
        }
        AssignConstraintTargetToHead();
        rigBuilder.Build();
    }

    private void AssignConstraintTargetToHead()
    {
        WeightedTransform weightedTransform = new WeightedTransform();
        weightedTransform.transform = hookTarget;
        weightedTransform.weight = 1.0f;
        headConstraint.data.sourceObjects = new WeightedTransformArray() { weightedTransform };
    }

    private void ThrowHook()
    {
        transform.forward = Vector3.ProjectOnPlane(hookTarget.position, Vector3.up);

        throwHook.Invoke();

        isHookThrown = false;
        currentOnHookSpeed = 0f;
        hangingDirection = Vector3.zero;
    }

    private void Update()
    {
        PointArmTowardsHookTarget();

        charController.Move(currentOnHookSpeed * Time.deltaTime * hangingDirection);

        if (IsHookTargetReached())
        {
            ExitState();
        }
    }

    private void PointArmTowardsHookTarget()
    {
        if (spineToFingerRig.weight < 1f && !isHookThrown)
        {
            spineToFingerRig.weight += rigAlignmentToHookTargetAcceleration * Time.deltaTime;
        }
        else
        {
            isHookThrown = true;
            spineToFingerRig.weight = 0f;
        }
    }

    private bool IsHookTargetReached()
    {
        return (hookTarget.position - transform.position).sqrMagnitude <= hookReachThreshold;
    }

    private void ExitState()
    {
        ChangeToHangingAnimation.Invoke(false);

        onAirState.enabled = true;
        spineToFingerRig.weight = 0f;
        this.enabled = false;
    }

    // Called through an animation event
    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        currentOnHookSpeed = speed;
        spineToFingerRig.weight = 0f;
        ChangeToHangingAnimation.Invoke(true);
    }
}
