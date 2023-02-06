using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierRunnerBT : BehaviourTree.Tree
{
    [SerializeField] BehaviourTreeSharedData sharedData;

    // Shared Tree Cached components
    private static DecisionMaker decisionMaker;
    public static DecisionMaker DecisionMaker => decisionMaker;
    private static NavMeshAgent navMeshAgent; 
    public static NavMeshAgent NavMeshAgent => navMeshAgent;
    private static CharacterAnimator characterAnimator;
    public static CharacterAnimator CharacterAnimator => characterAnimator;

    // Shared Tree Properties
    private static float patrolSpeed;
    public static float PatrolSpeed => patrolSpeed;
    private static float runningSpeed;
    public static float RunningSpeed => runningSpeed;
    private static bool isTargetInAttackRange;
    public static bool IsTargetInAttackRange { get { return isTargetInAttackRange; } set { isTargetInAttackRange = value; } }

    [Header("TaskPatrol Properties")]
    [SerializeField] Transform patrolParent;

    private void Awake()
    {
        patrolSpeed = sharedData.patrolSpeed;
        runningSpeed= sharedData.runningSpeed;
        navMeshAgent = GetComponent<NavMeshAgent>();
        decisionMaker = GetComponent<DecisionMaker>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    protected override Node SetUpTree()
    {
        // Order is important!!! first elements will always be prioritised. In this case TaskPatrol will be the standard behaviour if all else above returns a NodeState.FAILURE
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckTargetInAttackRange(),
                new TaskAttack(),
            }),
            new Sequence(new List<Node>
            {
                new CheckForInterestingThings(),
                new TaskGoToTarget(),
            }),
            new TaskPatrol(patrolParent),
        });

        return root;
    }
}
