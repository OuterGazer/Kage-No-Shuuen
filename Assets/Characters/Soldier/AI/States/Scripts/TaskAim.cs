using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class TaskAim : Node
{
    [Header("Fanciness")]
    [SerializeField] GameObject quiverArrow;
    [SerializeField] GameObject loadedArrow;
    [SerializeField] Transform bowstring;

    [Header("Animation Settings")]
    [SerializeField] float animationAcceleration = 0.05f;    
    [SerializeField] Transform leftHand;

    [Header("Rig Settings")]
    [SerializeField] Rig aimingRig;

    private Vector3 initialBowstringPosition = Vector3.zero;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;

    private bool isAimingAnimationPlaying = false;
    private bool isPullingBowstring = false;
    private bool isReadyToShoot = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        initialBowstringPosition = bowstring.localPosition;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (isAimingAnimationPlaying)
        {
            aimingRig.weight = (aimingRig.weight >= 1f) ? 1f : (aimingRig.weight += animationAcceleration * Time.deltaTime);

            if (isPullingBowstring)
            {
                bowstring.localPosition += bowstring.InverseTransformPoint(leftHand.TransformPoint(leftHand.localPosition));
            }

            if ((aimingRig.weight >= 1f) &&
                (isReadyToShoot))
            {
                transform.LookAt(target);
                navMeshAgent.speed = 0f;

                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.RUNNING;
            return state;
        }

        transform.LookAt(target);
        characterAnimator.PlayAimingAnimation(true);
        isAimingAnimationPlaying = true;
        Parent.SetData("interactionAnimation", true);

        state = NodeState.RUNNING;
        return state;
    }

    // Called from Animation Event
    internal void SpawnArrowInHand()
    {
        quiverArrow.SetActive(true);
    }

    internal void PullBowstring()
    {
        isPullingBowstring = true;
    }

    // Called from animation event
    internal void ReadyToShoot()
    {
        isPullingBowstring = false;
        isReadyToShoot = true;
    }

    // Called from Animation Event
    internal void SpawnArrowInBow()
    {
        quiverArrow.SetActive(false);
        loadedArrow.SetActive(true);
    }

    // Called from Animation Event
    internal void ReleaseBowstring()
    {
        isAimingAnimationPlaying = false;
        isReadyToShoot= false;
        bowstring.localPosition = initialBowstringPosition;
        loadedArrow.SetActive(false);
    }

    // Called from Animation Event
    internal void ReloadBow()
    {
        characterAnimator.PlayAimingAnimation(true);
        isAimingAnimationPlaying = true;
        Parent.SetData("interactionAnimation", true);
    }
}
