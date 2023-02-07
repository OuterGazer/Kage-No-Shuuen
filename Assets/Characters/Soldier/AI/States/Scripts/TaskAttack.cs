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
    

    private void Start()
    {
        navMeshAgent = ((SoldierRunnerBT)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierRunnerBT)belongingTree).CharacterAnimator;
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

        attackCounter += Time.deltaTime;
        if(attackCounter > timeBetweenAttacks)
        {
            characterAnimator.PlaySlashAnimation();
            attackCounter = 0f;

            if (GetData("interactionAnimation") == null)
            { Parent.Parent.SetData("interactionAnimation", true); }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
