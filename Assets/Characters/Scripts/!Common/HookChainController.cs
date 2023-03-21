using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            hookChainPrefab.transform.position += 30f * Time.deltaTime * transform.forward;
        }
    }

    public void ShootChain(Transform hookTarget)
    {
        this.hookTarget = hookTarget;
        StartCoroutine(SpawnChainLinks());
    }

    private IEnumerator SpawnChainLinks()
    {
        hookChainPrefab.SetActive(true);
        hookChainPrefab.transform.SetParent(null, true);

        isMovingTowardsTarget = true;

        yield return new WaitForSeconds(timeToReachTarget);

        isMovingTowardsTarget = false;

        //while(isMovingTowardsTarget)
        //{
            

        //    yield return new WaitForEndOfFrame();

        //    isMovingTowardsTarget = false;
        //}
    }
}
