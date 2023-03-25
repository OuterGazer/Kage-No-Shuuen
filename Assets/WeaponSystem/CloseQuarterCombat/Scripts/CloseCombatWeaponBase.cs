using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseCombatWeaponBase : MonoBehaviour
{
    [SerializeField] float damage = 0.51f;
    public float Damage => damage;

    [SerializeField] Collider col;

    protected bool IsSlashing { get; private set; }

    private void OnEnable()
    {
        if (IsSlashing) { DamageEnd(); }
    }

    // Called from WeaponController from an animation event
    internal void DamageStart()
    { 
        IsSlashing = true;
        if (col)
        { col.enabled = true; }
    }

    // Called from WeaponController from an animation event
    internal void DamageEnd()
    { 
        IsSlashing = false;
        if (col)
        { col.enabled = false; }
    }

    protected void PerformDamage(IDamagereceiver damageReceiver)
    {
        damageReceiver?.ReceiveDamage(damage);
    }
}
