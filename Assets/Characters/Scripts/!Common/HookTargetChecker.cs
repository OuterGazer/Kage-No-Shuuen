using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookTargetChecker : MonoBehaviour
{
    [Header("Target Checking Settings")]
    [SerializeField] float noNearbyTargetRefreshRate = 1f;
    [SerializeField] float nearbyTargetRefreshRate = 5f;
    [SerializeField] float checkRadius = 2f;
    [SerializeField] LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] LayerMask obstaclesLayerMask = Physics.DefaultRaycastLayers;

    private float targetCheckRefreshRate = 1f;
    private float checkCounter = 0f;

    private Transform hookTarget;
    public Transform HookTarget => hookTarget;
    private TrackedObject hookTargetIndicator;

    private bool canPerformHookThrow = false;
    public bool CanPerformHookThrow => canPerformHookThrow;

    private void RemoveTarget()
    {
        hookTargetIndicator?.SetIsIndicatorVisible(false);
        hookTarget = null;

        canPerformHookThrow = false;

        targetCheckRefreshRate = noNearbyTargetRefreshRate;
    }

    private void Start()
    {
        targetCheckRefreshRate = noNearbyTargetRefreshRate;
    }

    
    void Update()
    {
        checkCounter += Time.deltaTime;

        CheckForNearbyTarget();
    }

    private void CheckForNearbyTarget()
    {
        if (checkCounter >= (1 / targetCheckRefreshRate))
        {
            checkCounter = 0f;

            Collider[] targetCollider = Physics.OverlapSphere(transform.position, checkRadius, targetLayerMask);

            if (IsThereATargetNearby(targetCollider))
            {
                AssignTarget(targetCollider);
                CheckIfObstaclesBetweenCharacterAndTarget();
            }
            else if (IsAddedTargetGoneOutOfRange(targetCollider))
            {
                RemoveTarget();
            }
        }
    }

    private void AssignTarget(Collider[] targetCollider)
    {
        if(targetCollider.Length > 1)
        {
            FindClosestTarget(targetCollider);
        }

        if (hookTarget != targetCollider[0].transform) { RemoveTarget(); }
        
        if (!hookTarget)
        {
            
            hookTarget = targetCollider[0].transform;
            hookTargetIndicator = hookTarget.GetComponent<TrackedObject>();
            targetCheckRefreshRate = nearbyTargetRefreshRate;
        }
    }

    private void FindClosestTarget(Collider[] targetCollider)
    {
        for (int i = 1; i < targetCollider.Length; i++)
        {
            if (CurrentItemIsCloserThanPreviousItem(targetCollider, i))
            {
                Collider temp = targetCollider[i - 1];
                targetCollider[i - 1] = targetCollider[i];
                targetCollider[i] = temp;
                if (i > 1) { i--; }
            }
        }
    }

    private bool CurrentItemIsCloserThanPreviousItem(Collider[] targetCollider, int i)
    {
        return (targetCollider[i].transform.position - transform.position).sqrMagnitude < 
               (targetCollider[i - 1].transform.position - transform.position).sqrMagnitude;
    }

    private bool IsThereATargetNearby(Collider[] targetCollider)
    {
        return targetCollider.Length > 0;
    }

    private void CheckIfObstaclesBetweenCharacterAndTarget()
    {
        Vector3 tempHookDir = (hookTarget.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, tempHookDir, checkRadius, obstaclesLayerMask);

        SortDetectedCollidersByDistanceToCharacter(hits);

        if (hits.Length > 0 &&
           !hits[0].collider.CompareTag("HookTarget"))
        {
            hookTargetIndicator?.SetIsIndicatorVisible(false);
            canPerformHookThrow = false;
        }
        else
        {
            hookTargetIndicator?.SetIsIndicatorVisible(true);
            canPerformHookThrow = true;
        }
    }

    private void SortDetectedCollidersByDistanceToCharacter(RaycastHit[] hits)
    {
        for (int i = 1; i < hits.Length; i++)
        {
            if (CurrentColliderIsCloserThanPreviousCollider(hits, i))
            {
                RaycastHit temp = hits[i - 1];
                hits[i - 1] = hits[i];
                hits[i] = temp;
                if (i > 1) { i -= 2; }
            }
        }
    }

    private bool CurrentColliderIsCloserThanPreviousCollider(RaycastHit[] hits, int i)
    {
        return (hits[i].point - transform.position).sqrMagnitude < (hits[i - 1].point - transform.position).sqrMagnitude;
    }

    private bool IsAddedTargetGoneOutOfRange(Collider[] targetCollider)
    {
        return targetCollider.Length <= 0;
    }
}
