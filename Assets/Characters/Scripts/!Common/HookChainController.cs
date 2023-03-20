using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HookChainController : MonoBehaviour
{
    [SerializeField] GameObject hookChainPrefab;
    [SerializeField] float timeToReachTarget = 0.3f;

    private Transform hookTarget;
    [SerializeField] GameObject[] chainLinks;

    private bool isMovingTowardsTarget = false;

    private void Update()
    {
        if (isMovingTowardsTarget)
        {
            hookChainPrefab.transform.DOMove(hookTarget.position, timeToReachTarget, false);
        }
    }

    public void ShootChain()
    {
        StartCoroutine(SpawnChainLinks());
    }

    private IEnumerator SpawnChainLinks()
    {
        Instantiate(hookChainPrefab, transform.position, transform.rotation);
        isMovingTowardsTarget = true;

        while (true)
        {


            yield return new WaitForEndOfFrame();
        }
    }
}
