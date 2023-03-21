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

        CalculateTotalLinksToSpawn();

        isMovingTowardsTarget = true;

        yield return new WaitUntil(() => hasReachedTarget);

        isMovingTowardsTarget = false;
    }

    private int linkAmount;
    private void CalculateTotalLinksToSpawn()
    {
        float distanceToTarget = Vector3.Distance(transform.position, hookTarget.position);

        linkAmount = Mathf.CeilToInt(distanceToTarget / 0.06f); // 0.06f is approximately the size of a link

        StartCoroutine(SpawnLinksInWaves());
    }

    [SerializeField] int linksToSpawnPerFrame = 10;
    private IEnumerator SpawnLinksInWaves()
    {
        for (int i = 1; i < linkAmount; i++)
        {
            for (int j = 0; j <= linksToSpawnPerFrame; j++)
            {
                chainLinks[j + i].SetActive(true);
            }

            i += linksToSpawnPerFrame;

            yield return new WaitForEndOfFrame();
        }
    }

    public void DisableLinksAsCharacterMovesTowardTarget()
    {
        StartCoroutine(DisableLinks());
    }

    [SerializeField] int linksToDisablePerFrame = 2;
    [SerializeField] int framesToWaitToDisableNextBatch = 5;
    private IEnumerator DisableLinks()
    {
        for (int i = linkAmount; i > 1; i--)
        {
            for (int j = 0; j <= linksToDisablePerFrame; j++)
            {
                if((i - j) < 1) { continue; }
                chainLinks[i - j].SetActive(false);
            }

            i -= linksToDisablePerFrame;

            for(int z = 0; z < framesToWaitToDisableNextBatch; z++) 
            { yield return new WaitForEndOfFrame(); }
        }

        chainLinks[0].SetActive(false);
    }

    public void ReturnChainToGauntlet()
    {
        foreach (GameObject item in chainLinks)
        {
            if (item.activeSelf) { item.SetActive(false); }
            
            hasReachedTarget = false;
            hookChainPrefab.transform.SetParent(transform);
            hookChainPrefab.transform.localPosition = Vector3.zero;
            hookChainPrefab.transform.localRotation = Quaternion.identity;
            linkAmount = 0;
            hookChainPrefab.SetActive(false);
        }
    }
}
