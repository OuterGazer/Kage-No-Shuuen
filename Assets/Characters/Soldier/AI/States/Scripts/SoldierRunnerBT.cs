using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierRunnerBT : BehaviourTree.Tree
{
    [Header("Tree Sub Roots")]
    [SerializeField] Node localMainRoot;

    // Shared Tree Cached components
    private static DecisionMaker decisionMaker;
    public static DecisionMaker DecisionMaker => decisionMaker;
    private static NavMeshAgent navMeshAgent; 
    public static NavMeshAgent NavMeshAgent => navMeshAgent;
    private static CharacterAnimator characterAnimator;
    public static CharacterAnimator CharacterAnimator => characterAnimator;

    // Shared Tree Properties
    private static float patrolSpeed = 2f;
    public static float PatrolSpeed => patrolSpeed;
    private static float runningSpeed = 5f;
    public static float RunningSpeed => runningSpeed;
    private static bool isTargetInAttackRange;
    public static bool IsTargetInAttackRange { get { return isTargetInAttackRange; } set { isTargetInAttackRange = value; } }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        decisionMaker = GetComponent<DecisionMaker>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    protected override Node SetUpTree()
    {
        return localMainRoot;
    }
}