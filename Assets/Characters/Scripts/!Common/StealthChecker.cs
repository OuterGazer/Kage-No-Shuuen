using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthChecker : MonoBehaviour
{
    [Header("Target Checking Settings")]
    [SerializeField] float noNearbyTargetRefreshRate = 1f;
    [SerializeField] float nearbyTargetRefreshRate = 5f;
    [SerializeField] float checkRadius = 2f;
    [SerializeField] LayerMask targetLayerMask = Physics.DefaultRaycastLayers;

    private float targetCheckRefreshRate = 1f;
    private float checkCounter = 0f;

    DecisionMaker targetDecisionMaker;

    private bool isSeenByTarget = false;

    public void CharacterIsSeen(Transform transform)
    {
        isSeenByTarget = true;
    }
    public void CharacterIsInStealth()
    {
        if (isSeenByTarget)
        {
            RemoveTarget();
        }
    }

    private void RemoveTarget()
    {
        isSeenByTarget = false;
        targetDecisionMaker.OnPlayerSeen.RemoveListener(CharacterIsSeen);
        targetDecisionMaker.OnTargetLost.RemoveListener(CharacterIsInStealth);
        targetDecisionMaker = null;

        targetCheckRefreshRate = noNearbyTargetRefreshRate;
    }

    private void Start()
    {
        targetCheckRefreshRate = noNearbyTargetRefreshRate;
    }

    
    void Update()
    {
        checkCounter += Time.deltaTime;

        if(checkCounter >= (1 / targetCheckRefreshRate))
        {
            checkCounter = 0f;

            Collider[] targetCollider = Physics.OverlapSphere(transform.position, checkRadius, targetLayerMask);

            if((targetCollider.Length > 0) &&
                (!targetDecisionMaker))
            {
                targetCheckRefreshRate = nearbyTargetRefreshRate;

                targetDecisionMaker = targetCollider[0].GetComponentInParent<DecisionMaker>();
                targetDecisionMaker.OnPlayerSeen.AddListener(CharacterIsSeen);
                targetDecisionMaker.OnTargetLost.AddListener(CharacterIsInStealth);
            }
            else if (targetDecisionMaker && targetCollider.Length <= 0)
            {
                RemoveTarget();
            }

            if(!isSeenByTarget && targetDecisionMaker)
            { Debug.Log("Stealth Kill is possible!"); }
        }
    }


}
