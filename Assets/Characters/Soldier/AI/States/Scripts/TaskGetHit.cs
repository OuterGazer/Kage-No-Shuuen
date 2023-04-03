using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGetHit : Node
{
    [SerializeField] float pushLengthOnHit = 1f;
    [SerializeField] AudioClip hitSound;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private DamageableWithLife damageable;
    private Transform player;
    private AudioSource audioSource;

    private bool isAnimationRunning = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        damageable = ((SoldierBehaviour)belongingTree).Damageable;
        damageable.OnGettingHit.AddListener(SetGettingHit);
        player = ((SoldierBehaviour)belongingTree).Player;
        audioSource = ((SoldierBehaviour)belongingTree).AudioSource;
    }

    private void OnDestroy()
    {
        damageable.OnGettingHit.RemoveListener(SetGettingHit);
    }

    public override NodeState Evaluate()
    {
        if(isAnimationRunning)
        {
            state = NodeState.RUNNING; 
            return state;
        }

        object h = GetData("justGotHit");
        if(h == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        characterAnimator.PlayHitAnimation();
        isAnimationRunning = true;
        navMeshAgent.Move(pushLengthOnHit * -transform.forward);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(hitSound);

        object t = GetData("target");
        if(t == null)
        {
            Parent.Parent.SetData("target", player);
        }

        state = NodeState.SUCCESS;
        return state;
    }

    // Called from event raised in DamageableWithLife
    private void SetGettingHit()
    {
        Parent.SetData("justGotHit", true);
    }

    // Called from animation event in hit animation
    private void ExitHitState()
    {
        isAnimationRunning = false;
        ClearData("justGotHit");
    }
}
