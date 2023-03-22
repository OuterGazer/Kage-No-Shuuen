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

            //if(name.Contains("Bomb"))
            //{
            //    IDamagereceiver selfReceiver = gameObject.GetComponentInParent<WeaponController>().GetComponentInChildren<IDamagereceiver>();
            //    PerformDamage(selfReceiver);
            //}
        }
    }
}
