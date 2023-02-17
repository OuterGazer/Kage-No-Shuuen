using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskShoot : Node
{
    [SerializeField] ShootingWeapon shootingWeapon;
    [SerializeField] float timeBetweenShots = 3f;
    [SerializeField] GameObject loadedArrow;

    private float shootingCounter = 0f;

    private CharacterAnimator characterAnimator;

    private bool isShootingAnimationPlaying = false;


    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
    }

    public override NodeState Evaluate()
    {
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
}
