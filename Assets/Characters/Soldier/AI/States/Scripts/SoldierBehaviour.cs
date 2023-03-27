using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierBehaviour : BehaviourTree.Tree, IObjectPoolNotifier
{
    [SerializeField] SoldierType type = SoldierType.Runner;
    public SoldierType Type => type;

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
    public DamageableWithLife Damageable => damageable;

    // Shared Tree Properties
    [SerializeField] float patrolSpeed = 2f;
    public float PatrolSpeed => patrolSpeed;

    //Miscellaneous
    [SerializeField] GameObject smokeBomb;

    protected override Node SetUpTree()
    {
        return localMainRoot;
    }

    public void OnEnqueuedToPool()
    {
        // TODO: for example deactivate the ragdoll, set life again to full points, etc.
        FindObjectOfType<WaveController>().currentWaveSoldiers.Remove(transform);
        NavMeshAgent.Warp(Vector3.zero);
        GetComponent<TaskDie>().SetIsAnimationRunning(false);
    }

    public void OnCreatedOrDequeuedFromPool(bool isCreated, Transform patrolParent) //true: is created first time -- false: is just activated
    {
        // This gets called before spawning, I may assign patrol parent here most probably and also directly the player target.
        if (isCreated) { player = FindObjectOfType<StateController>().transform; }

        damageable.GetComponent<Collider>().enabled = true;
        NavMeshAgent.enabled = true;
        this.enabled = true;

        TaskPatrol patrol = GetComponent<TaskPatrol>();
        TaskGuardPosition guardPos = GetComponent<TaskGuardPosition>();

        if (patrol)
        {
            patrol.SetPatrol(patrolParent);
        }
        else
        {
            guardPos.SetGuard(patrolParent);
        }

        NavMeshAgent.Warp(patrolParent.transform.position);

        GameObject smokeBomb = Instantiate(this.smokeBomb, transform.position, Quaternion.identity);
        Destroy(smokeBomb, 5f);
    }
}
