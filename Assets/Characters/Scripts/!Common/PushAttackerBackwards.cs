using System;
using UnityEngine;
using UnityEngine.AI;

public class PushAttackerBackwards : MonoBehaviour
{
    // TODO: have pushback not be an instant teleportation
    // TODO: have a system that avoids taking damage while blocking. Just in case, sometimes player gets hit, sometimes not.

    private void OnTriggerEnter(Collider other)
    {
        string weaponBeingBlocked = other.transform.parent?.parent?.parent?.name;
        if (IsBlockedWeaponAProjectile(weaponBeingBlocked)) { return; }

        bool isPhysicalWeapon = weaponBeingBlocked.Contains("Weapon");

        if (isPhysicalWeapon)
        {
            PushAttacker(other);
        }
    }

    private static bool IsBlockedWeaponAProjectile(string weaponBeingBlocked)
    {
        return String.IsNullOrEmpty(weaponBeingBlocked);
    }

    private void PushAttacker(Collider other)
    {
        other.GetComponentInParent<NavMeshAgent>()?.Move(1f * transform.forward);
    }
}
