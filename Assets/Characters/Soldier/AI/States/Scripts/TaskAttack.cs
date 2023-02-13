using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskAttack : Node
{
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] float attackCounter = 0.5f; // Serialized for testing purposes

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;

    private bool isAttackAnimationRunning = false;
    

    private void Start()
    {
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
    }

    public override NodeState Evaluate()
    {
        navMeshAgent.speed = 0f;

        Transform target = (Transform) GetData("target");
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        ClearData("searchTarget");

        attackCounter += Time.deltaTime;
        if(attackCounter > timeBetweenAttacks)
        {
            characterAnimator.PlaySlashAnimation();
            attackCounter = 0f;

            isAttackAnimationRunning = true;
            Parent.Parent.SetData("interactionAnimation", isAttackAnimationRunning);
        }

        state = NodeState.RUNNING;
        return state;
    }

    // Called from an animation event
    private void ExitCloseCombatState()
    {
        isAttackAnimationRunning = false;
        ClearData("interactionAnimation");
    }
}
