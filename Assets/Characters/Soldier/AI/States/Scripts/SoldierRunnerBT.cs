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
    [Header("Shared Tree Cached Components")]
    [SerializeField] DecisionMaker decisionMaker;
    public DecisionMaker DecisionMaker => decisionMaker;
    [SerializeField] NavMeshAgent navMeshAgent; 
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    [SerializeField] CharacterAnimator characterAnimator;
    public CharacterAnimator CharacterAnimator => characterAnimator;

    // Shared Tree Properties
    

    protected override Node SetUpTree()
    {
        return localMainRoot;
    }
}
