using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class TaskShoot : Node
{
    [Header("Shooting Settings")]
    [SerializeField] ShootingWeapon shootingWeapon;
    [SerializeField] float timeBetweenShots = 3f;
    [SerializeField] GameObject loadedArrow;

    [Header("Rig Settings")]
    [SerializeField] Rig aimingRig;

    private float shootingCounter = 0f;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;

    private bool isShootingAnimationPlaying = false;


    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        shootingCounter += Time.deltaTime;

        if(shootingCounter <= timeBetweenShots)
        {
            state = NodeState.RUNNING;
            return state;
        }

        shootingCounter = 0f;
        characterAnimator.PlayShootingAnimation();
        isShootingAnimationPlaying = true;

        if (isShootingAnimationPlaying)
        { state = NodeState.RUNNING; }
        else
        { state = NodeState.SUCCESS; }

        return state;
    }

    // Called from an animation event
    internal void ShootBow()
    {
        shootingWeapon?.Shoot();
        ClearData("interactionAnimation");
        isShootingAnimationPlaying = false;
    }

    private void EraseInterestingTarget()
    {
        object t = GetData("target");
        if (t == null) { return; }

        if (state == NodeState.RUNNING)
        {
            ClearData("target");
            StopAiming();
        }
    }

    private void StopAiming()
    {
        characterAnimator.PlayAimingAnimation(false);

        if (loadedArrow.activeInHierarchy)
        {
            loadedArrow.SetActive(false);
        }

        aimingRig.weight = 0f;
    }
}
