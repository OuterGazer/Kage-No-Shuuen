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
    [SerializeField] AudioClip shootingSound;

    private float shootingCounter = 0f;

    private CharacterAnimator characterAnimator;
    private AudioSource audioSource;

    private bool isShootingAnimationPlaying = false;


    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        audioSource = ((SoldierBehaviour)belongingTree).AudioSource;
    }

    public override NodeState Evaluate()
    {
        shootingCounter += Time.deltaTime;

        Transform target = (Transform)GetData("target");
        if (!target)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (isShootingAnimationPlaying)
        { 
            state = NodeState.RUNNING;
            return state;
        }

        if (shootingCounter <= timeBetweenShots)
        {
            state = NodeState.RUNNING;
            return state;
        }

        characterAnimator.PlayShootingAnimation();
        isShootingAnimationPlaying = true;
        shootingCounter = 0f;

        state = NodeState.SUCCESS;
        return state;
    }

    // Called from an animation event
    internal void ShootBow()
    {
        shootingWeapon?.Shoot();
        audioSource.PlayOneShot(shootingSound);
        ClearData("interactionAnimation");
        isShootingAnimationPlaying = false;
    }
}
