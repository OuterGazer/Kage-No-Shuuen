using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskGetHit : Node
{
    [SerializeField] float pushLengthOnHit = 1f;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private DamageableWithLife damageable;

    private bool isAnimationRunning = false;

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        damageable = ((SoldierBehaviour)belongingTree).DamageableWithLife;
        damageable.OnGettingHit.AddListener(SetGettingHit);
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
