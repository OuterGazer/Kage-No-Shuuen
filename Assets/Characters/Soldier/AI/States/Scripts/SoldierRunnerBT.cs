using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using System.ComponentModel.Design.Serialization;

public class SoldierRunnerBT : BehaviourTree.Tree
{
    // TODO: look to serialize static fields in the editor

    // Shared Tree Cached components
    //private static Weapon weapon;
    //public static Weapon Weapon => weapon;
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
    private static bool isTargetInAttackRange = false;
    public static bool IsTargetInAttackRange { get { return isTargetInAttackRange; } set { isTargetInAttackRange = value; } }

    [Header("TaskPatrol Specific Properties")]
    [SerializeField] Transform patrolParent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        decisionMaker = GetComponent<DecisionMaker>();
        //weapon = GetComponent<Weapon>();
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
