using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookChainController : MonoBehaviour
{
    [SerializeField] GameObject hookChainPrefab;
    [SerializeField] GameObject[] chainLinks;
    [SerializeField] float timeToReachTarget = 0.3f;

    private Transform hookTarget;
    private Vector3 targetDirection = Vector3.zero;

    private bool isMovingTowardsTarget = false;

    private void Update()
    {
        if (isMovingTowardsTarget)
        {
            hookChainPrefab.transform.position += 30f * Time.deltaTime * targetDirection;
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

        yield return new WaitForSeconds(timeToReachTarget);

        isMovingTowardsTarget = false;
    }

    public void ReturnChainToGauntlet()
    {
        hookChainPrefab.transform.SetParent(transform);
        hookChainPrefab.transform.localPosition = Vector3.zero;
        hookChainPrefab.transform.localRotation = Quaternion.identity;
        hookChainPrefab.SetActive(false);
    }
}
