using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class TaskDodge : Node
{
    [SerializeField] float dodgingDistance = 2f;
    [SerializeField] float dodgingSpeed = 4f;

    CharacterAnimator characterAnimator;
    NavMeshAgent navMeshAgent;
    DodgeStateBehaviour dodgeBehaviour;

    private Vector3 facingDirection = Vector3.zero;
    private Vector3 dodgingEndPoint= Vector3.zero;

    private bool isDodging = false;

    private void Start()
    {
        characterAnimator = ((SoldierBehaviour)belongingTree).CharacterAnimator;
        navMeshAgent = ((SoldierBehaviour)belongingTree).NavMeshAgent; 
        dodgeBehaviour = characterAnimator.Animator.GetBehaviour<DodgeStateBehaviour>();
        dodgeBehaviour.ExitState.AddListener(ExitDodgingState);
    }

    private void OnDestroy()
    {
        dodgeBehaviour.ExitState.RemoveListener(ExitDodgingState);
    }

    public override NodeState Evaluate()
    {
        if(isDodging)
        {
            transform.forward = facingDirection;
            navMeshAgent.speed = dodgingSpeed;
            navMeshAgent.destination = dodgingEndPoint;

            state = NodeState.RUNNING; 
            return state;
        }

        Transform target = (Transform)GetData("target");
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        object b = GetData("hasDodged"); // Have we dodged, but not yet attacked? Then don't dodge again. To avoid a constant loop of dodging and throwing weapons
        if (b != null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        facingDirection = -transform.forward;
        dodgingEndPoint = transform.position + (facingDirection * dodgingDistance);
        characterAnimator.PlayDodgeAnimation();

        Parent.Parent.SetData("interactionAnimation", true);
        Parent.Parent.SetData("hasDodged", true);
        isDodging = true;
        
        state = NodeState.RUNNING;
        return state;
    }

    private void ExitDodgingState()
    {
        isDodging = false;
        ClearData("interactionAnimation");
        transform.forward = -facingDirection;
    }
}
