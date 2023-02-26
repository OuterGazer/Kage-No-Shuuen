using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseCombatWeaponBase : MonoBehaviour
{
    [SerializeField] float damage = 0.51f;
    public float Damage => damage;

    [SerializeField] Collider col;

    protected bool IsSlashing { get; private set; }

    // Called from WeaponController from an animation event
    internal void DamageStart()
    { 
        IsSlashing = true;
        col.enabled = true;
    }

    // Called from WeaponController from an animation event
    internal void DamageEnd()
    { 
        IsSlashing = false;
        col.enabled = false;
    }

    protected void PerformDamage(IDamagereceiver damageReceiver)
    {
        damageReceiver?.ReceiveDamage(damage);
    }
}
