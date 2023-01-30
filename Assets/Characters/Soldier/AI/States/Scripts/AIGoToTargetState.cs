using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGoToTargetState : AIBaseState
{
    [SerializeField] Transform target;

    private NavMeshAgent navMeshAgent;

    private void OnEnable()
    {
        if(navMeshAgent.isStopped)
            navMeshAgent.isStopped = false;
    }

    protected override void InternalAwake()
    {
        base.InternalAwake();

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void InternalUpdate()
    {
        base.InternalUpdate();

        if (target) 
            { navMeshAgent.destination = target.position; }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDisable()
    {
        target = null;
    }
}
