using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskAttack : Node
{
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] float attackCounter = 0f; // Serialized for testing purposes
    [SerializeField] bool shouldPerformHeavyAttack = false;

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
        attackCounter += Time.deltaTime;

        if (isAttackAnimationRunning)
        { 
            navMeshAgent.speed = 0f;

            state = NodeState.RUNNING;
            return state;
        }
        

        Transform target = (Transform) GetData("target");
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        ClearData("searchTarget");

        if(attackCounter > timeBetweenAttacks)
        {
            if (!shouldPerformHeavyAttack)
            { characterAnimator.PlaySlashAnimation(); }
            else 
            { characterAnimator.PlayHeavySlashAnimation(); }
            attackCounter = 0f;

            isAttackAnimationRunning = true;
            Parent.Parent.SetData("interactionAnimation", true);
        }

        state = NodeState.RUNNING;
        return state;
    }

    // Called from an animation event
    private void ExitCloseCombatState()
    {
        isAttackAnimationRunning = false;
        ClearData("interactionAnimation");
        ClearData("hasDodged");
    }
}
