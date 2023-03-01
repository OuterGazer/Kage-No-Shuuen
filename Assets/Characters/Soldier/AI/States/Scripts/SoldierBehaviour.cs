using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierBehaviour : BehaviourTree.Tree
{
    [Header("Tree Sub Roots")]
    [SerializeField] Node localMainRoot;

    // Shared Tree Cached components
    [Header("Shared Tree Cached Components")]
    [SerializeField] WeaponController weaponController;
    public WeaponController WeaponController => weaponController;
    [SerializeField] DecisionMaker decisionMaker;
    public DecisionMaker DecisionMaker => decisionMaker;
    [SerializeField] NavMeshAgent navMeshAgent; 
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    [SerializeField] CharacterAnimator characterAnimator;
    public CharacterAnimator CharacterAnimator => characterAnimator;
    [SerializeField] Transform player;
    public Transform Player => player;
    [SerializeField] DamageableWithLife damageable;
    public DamageableWithLife DamageableWithLife => damageable;

    // Shared Tree Properties
    [SerializeField] float patrolSpeed = 2f;
    public float PatrolSpeed => patrolSpeed;

    protected override Node SetUpTree()
    {
        return localMainRoot;
    }
}
