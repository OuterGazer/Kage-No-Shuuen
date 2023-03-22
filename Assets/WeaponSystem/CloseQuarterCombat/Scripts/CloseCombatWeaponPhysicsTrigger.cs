using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombatWeaponPhysicsTrigger : CloseCombatWeaponBase
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(tag) && IsSlashing)
        {
            IDamagereceiver damageReceiver = other.GetComponent<IDamagereceiver>();
            PerformDamage(damageReceiver);
        }
    }
}
