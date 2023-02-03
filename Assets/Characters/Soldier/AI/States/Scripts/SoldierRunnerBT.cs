using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierRunnerBT : BehaviourTree.Tree
{
    // Shared Tree Properties
    private static NavMeshAgent navMeshAgent; // TODO: look to serialize static fields in the editor
    public static NavMeshAgent NavMeshAgent => navMeshAgent;
    private static float patrolSpeed = 3f;
    public static float PatrolSpeed => patrolSpeed;
    private static float runningSpeed = 6f;
    public static float RunningSpeed => runningSpeed;

    [Header("TaskPatrol Specific Properties")]
    [SerializeField] Transform patrolParent;

    private void Awake()
    {
        navMeshAgent= GetComponent<NavMeshAgent>();
    }

    protected override Node SetUpTree()
    {
        Node root = new TaskPatrol(patrolParent);
        return root;
    }
}
