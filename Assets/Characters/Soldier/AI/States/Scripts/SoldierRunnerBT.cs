using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
using System.ComponentModel.Design.Serialization;

public class SoldierRunnerBT : BehaviourTree.Tree
{
    // Shared Tree Properties
    private static DecisionMaker decisionMaker;
    public static DecisionMaker DecisionMaker => decisionMaker;
    private static NavMeshAgent navMeshAgent; // TODO: look to serialize static fields in the editor
    public static NavMeshAgent NavMeshAgent => navMeshAgent;
    private static float patrolSpeed = 2f;
    public static float PatrolSpeed => patrolSpeed;
    private static float runningSpeed = 5f;
    public static float RunningSpeed => runningSpeed;

    [Header("TaskPatrol Specific Properties")]
    [SerializeField] Transform patrolParent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        decisionMaker = GetComponent<DecisionMaker>();
    }

    protected override Node SetUpTree()
    {
        // Order is important!!! first elements will always be prioritised. In this case TaskPatrol will be the standard behaviour if all else above returns a NodeState.FAILURE
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckForInterestingThings(decisionMaker),
                new TaskGoToTarget(),
            }),
            new TaskPatrol(patrolParent),
        });

        return root;
    }
}
