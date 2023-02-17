using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System.Security.Claims;
using UnityEngine.Animations.Rigging;

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
    //[SerializeField] RigBuilder rigBuilder;
    [SerializeField] Rig aimingRig;
    //[SerializeField] ChainIKConstraint chainIKConstraint;
    private Vector3 initialBowstringPosition;

    private CharacterAnimator characterAnimator;

    private bool isAimingAnimationPlaying = false;

    private void Start()
    {
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
            if (aimingRig.weight >= 1f)
            {
                transform.LookAt(target);

                state = NodeState.SUCCESS;
                return state;
            }

            aimingRig.weight = (aimingRig.weight >= 1f) ? 1f : (aimingRig.weight += animationAcceleration * Time.deltaTime);
            
            bowstring.localPosition = bowstring.InverseTransformPoint(leftHand.TransformPoint(leftHand.localPosition));

            state = NodeState.RUNNING;
            return state;
        }

        characterAnimator.PlayAimingAnimation(true);
        isAimingAnimationPlaying = true;
        transform.LookAt(target);
        Parent.SetData("interactionAnimation", true);

        //chainIKConstraint.data.target = target;
        //rigBuilder.Build();

        state = NodeState.RUNNING;
        return state;
    }

    // Called from Animation Event
    internal void SpawnArrowInHand()
    {
        quiverArrow.SetActive(true);
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
        bowstring.localPosition = initialBowstringPosition;
        loadedArrow.SetActive(false);
    }

    // Called from Animation Event
    internal void ReloadBow()
    {
        characterAnimator.PlayAimingAnimation(true);
        initialBowstringPosition = bowstring.localPosition;
    }
}
