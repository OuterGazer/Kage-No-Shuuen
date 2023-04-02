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
    [SerializeField] AudioClip playerIsDeadClip;

    private NavMeshAgent navMeshAgent;
    private CharacterAnimator characterAnimator;
    private DecisionMaker decisionMaker;
    private AttackStateBehaviour attackStateBehaviour;
    private AudioSource audioSource;

    private bool isAttackAnimationRunning = false;    

    private void Start()
    {
        audioSource = ((SoldierBehaviour)belongingTree).AudioSource;
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent;
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        decisionMaker = ((SoldierBehaviour)belongingTree).DecisionMaker;
        decisionMaker.OnTargetLost.AddListener(EraseAttackTarget);
        attackStateBehaviour = characterAnimator.Animator.GetBehaviour<AttackStateBehaviour>();
        attackStateBehaviour.ExitState.AddListener(ExitCloseCombatState);
    }

    private void OnDestroy()
    {
        decisionMaker.OnTargetLost.RemoveListener(EraseAttackTarget);
        attackStateBehaviour.ExitState.RemoveListener(ExitCloseCombatState);
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

    public void EraseAttackTarget()
    {
        if (state == NodeState.RUNNING)
        {
            ClearData("target");
            audioSource.PlayOneShot(playerIsDeadClip);
            SoundManager.Instance.ReturnToStealthMusic(1f);
            TrackedObject[] indicators = GetComponentsInChildren<TrackedObject>();

            if(indicators.Length < 1) { return; }

            foreach(TrackedObject item in indicators)
            {
                item.HideIndicator();
            }
        }
    }
}
