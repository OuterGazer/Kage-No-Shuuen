using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskDie : Node
{
    private CharacterAnimator characterAnimator;
    private DamageableWithLife damageable;

    private bool isAnimationRunning = false;

    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        damageable = ((SoldierBehaviour)belongingTree).DamageableWithLife;
        damageable.OnDying.AddListener(SetIsDead);
    }

    private void OnDestroy()
    {
        damageable.OnDying.RemoveListener(SetIsDead);
    }

    public override NodeState Evaluate()
    {
        if (isAnimationRunning)
        {
            state = NodeState.RUNNING;
            return state;
        }

        object h = GetData("isDead");
        if (h == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        characterAnimator.PlayDeathAnimation();
        isAnimationRunning = true;
        state = NodeState.SUCCESS;
        return state;
    }

    // Called from event raised in DamageableWithLife
    private void SetIsDead()
    {
        Parent.SetData("isDead", true);
        //Destroy(damageable.gameObject);
        damageable.gameObject.SetActive(false);
    }
}
