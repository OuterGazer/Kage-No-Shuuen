using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookChainController : MonoBehaviour
{
    [SerializeField] GameObject hookChainPrefab;
    [SerializeField] GameObject[] chainLinks;

    private Transform hookTarget;
    private Vector3 targetDirection = Vector3.zero;

    private bool isMovingTowardsTarget = false;
    private bool hasReachedTarget = false;

    private void Update()
    {
        if (isMovingTowardsTarget)
        {
            hookChainPrefab.transform.position += 30f * Time.deltaTime * targetDirection;

            hasReachedTarget = CheckIfChainHasReachedTarget();
        }
    }

    private bool CheckIfChainHasReachedTarget()
    {
        if((hookTarget.position - hookChainPrefab.transform.position).sqrMagnitude <= 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ShootChain(Transform hookTarget)
    {
        this.hookTarget = hookTarget;        
        hookChainPrefab.transform.LookAt(this.hookTarget);
        targetDirection = (this.hookTarget.position - hookChainPrefab.transform.position).normalized;

        StartCoroutine(SpawnChainLinks());
    }

    private IEnumerator SpawnChainLinks()
    {
        hookChainPrefab.SetActive(true);
        hookChainPrefab.transform.SetParent(null, true);

        isMovingTowardsTarget = true;

        yield return new WaitUntil(() => hasReachedTarget);

        isMovingTowardsTarget = false;
    }

    public void ReturnChainToGauntlet()
    {
        hasReachedTarget = false;
        hookChainPrefab.transform.SetParent(transform);
        hookChainPrefab.transform.localPosition = Vector3.zero;
        hookChainPrefab.transform.localRotation = Quaternion.identity;
        hookChainPrefab.SetActive(false);
    }
}
