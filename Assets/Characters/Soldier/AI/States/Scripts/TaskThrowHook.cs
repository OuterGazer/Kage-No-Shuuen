using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.Animations.Rigging;

public class TaskThrowHook : Node
{
    private Transform hookTarget;

    private CharacterAnimator characterAnimator;

    [Header("Rig Characteristics")]
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] ChainIKConstraint[] shoulderToFingerConstraints;
    [SerializeField] MultiAimConstraint headConstraint;

    private Rig spineToFingerRig;
    private Vector3 hangingDirection;

    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        characterAnimator.hookHasArrivedAtTarget.AddListener(MoveCharacterToHookTarget);
        spineToFingerRig = GetComponentInChildren<Rig>();
        Parent.SetData("hangingDirection", hangingDirection);
    }

    private void OnDestroy()
    {
        characterAnimator.hookHasArrivedAtTarget.RemoveListener(MoveCharacterToHookTarget);
    }

    public override NodeState Evaluate()
    {
        object b = GetData("isMovingToHookTarget");
        if (b != null)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        hookTarget = (Transform)GetData("hookTarget");
        if (!hookTarget)
        {
            state = NodeState.FAILURE;
            return state;
        }

        PerformHookThrowing();

        state = NodeState.SUCCESS;
        return state;
    }

    private void PerformHookThrowing()
    {
        SetTargetToRigChain();
        ThrowHook();
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

        characterAnimator.HaveCharacterThrowHook();

        Parent.SetData("isHookThrown", false);
        hangingDirection = Vector3.zero;

        Parent.SetData("isMovingToHookTarget", true);
    }

    // Called from animation event
    public void MoveCharacterToHookTarget()
    {
        hangingDirection = (hookTarget.position - transform.position).normalized;
        transform.up = hangingDirection;
        spineToFingerRig.weight = 0f;
        characterAnimator.TransitionToOrFromHooked(true);
    }
}
