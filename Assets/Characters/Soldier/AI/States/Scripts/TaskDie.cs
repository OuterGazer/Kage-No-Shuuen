using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using UnityEngine.Events;

public class TaskDie : Node
{
    private CharacterAnimator characterAnimator;
    private DamageableWithLife damageable;

    public UnityEvent OnTargetLost;

    private bool isAnimationRunning = false;
    public void SetIsAnimationRunning(bool isAnimationRunning)
    {
        this.isAnimationRunning = isAnimationRunning;

        ClearAllSharedData();
        belongingTree.enabled = false;
    }

    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        damageable = ((SoldierBehaviour)belongingTree).Damageable;
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

    private void ClearAllSharedData()
    {
        ClearData("isDead");
        ClearData("target");
        ClearData("searchTarget");
        ClearData("interactionAnimation");
        ClearData("hookTarget");
        ClearData("isMovingToHookTarget");
        ClearData("hangingDirection");
    }

    // Called from event raised in DamageableWithLife
    private void SetIsDead()
    {
        Parent.Parent.SetData("isDead", true);
        //Destroy(damageable.gameObject);
        //damageable.gameObject.SetActive(false);
        damageable.gameObject.GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        StartCoroutine(DelayLifeBarDisappearence());
    }

    private IEnumerator DelayLifeBarDisappearence()
    {
        yield return new WaitForSeconds(1f);

        OnTargetLost.Invoke();
    }
}
