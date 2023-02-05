using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombatWeaponMultipleRaycast : CloseCombatWeaponBase
{
    [SerializeField] Transform edgeNotch;
    [SerializeField] Transform tip;
    [SerializeField] int raysPerDistance = 30;
    [SerializeField] LayerMask damageableLayerMask = Physics.DefaultRaycastLayers;

    float weaponLength;
    Vector3 oldPosition;
    Vector3 oldDirection;

    private void Start()
    {
        weaponLength = tip.transform.localPosition.z - edgeNotch.transform.localPosition.z;
        weaponOwnerTag = transform.parent.parent.gameObject.tag; // prefab root is 2 levels above the weapon in the hierarchy.
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = edgeNotch.position;
        Vector3 currentDirection = edgeNotch.forward;

        DealDamageThroughRaycasting(currentPosition, currentDirection);

        oldPosition = currentPosition;
        oldDirection = currentDirection;
    }

    private void DealDamageThroughRaycasting(Vector3 currentPosition, Vector3 currentDirection)
    {
        if (IsSlashing)
        {
            Vector3 oldTipPosition, currentTipPosition;
            UpdateTipPosition(currentPosition, currentDirection, out oldTipPosition, out currentTipPosition);
            int raysToCast = CalculateRaysToCast(oldTipPosition, currentTipPosition);
            PerformRaycasts(currentPosition, oldTipPosition, currentTipPosition, raysToCast);
        }
    }

    private void UpdateTipPosition(Vector3 currentPosition, Vector3 currentDirection, out Vector3 oldTipPosition, out Vector3 currentTipPosition)
    {
        oldTipPosition = oldPosition + Quaternion.LookRotation(oldDirection) * (Vector3.forward * weaponLength);
        currentTipPosition = currentPosition + Quaternion.LookRotation(currentDirection) * (Vector3.forward * weaponLength);
    }

    private int CalculateRaysToCast(Vector3 oldTipPosition, Vector3 currentTipPosition)
    {
        return CalculateAmountOfRaysAccordingToDistance(oldTipPosition, currentTipPosition);
    }

    private int CalculateAmountOfRaysAccordingToDistance(Vector3 oldTipPosition, Vector3 currentTipPosition)
    {
        return Mathf.CeilToInt((oldTipPosition - currentTipPosition).magnitude * raysPerDistance);
    }

    private void PerformRaycasts(Vector3 currentPosition, Vector3 oldTipPosition, Vector3 currentTipPosition, int raysToCast)
    {
        for (int i = 0; i < raysToCast; i++)
        {
            Vector3 origin, destination;
            CalculateRaycastOriginAndEnd(currentPosition, oldTipPosition, currentTipPosition, raysToCast, i, out origin, out destination);

            PerformRaycast(origin, destination);
        }
    }

    private void CalculateRaycastOriginAndEnd(Vector3 currentPosition, Vector3 oldTipPosition, Vector3 currentTipPosition, int raysToCast, int i, out Vector3 origin, out Vector3 destination)
    {
        origin = Vector3.Lerp(oldPosition, currentPosition, (float)i / (float)raysToCast);
        destination = Vector3.Lerp(oldTipPosition, currentTipPosition, (float)i / (float)raysToCast);
        Debug.DrawLine(origin, destination, Color.red, 1f);
    }

    private void PerformRaycast(Vector3 origin, Vector3 destination)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, destination - origin, out hit, weaponLength, damageableLayerMask))
        {
            Collider colliderHit = hit.collider;
            if (!colliderHit.CompareTag(weaponOwnerTag))
            {
                IDamagereceiver damageReceiver = colliderHit.GetComponent<IDamagereceiver>();
                PerformDamage(damageReceiver);
            }
            
        }
    }
}