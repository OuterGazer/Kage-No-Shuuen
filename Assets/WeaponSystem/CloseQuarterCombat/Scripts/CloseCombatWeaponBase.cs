using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseCombatWeaponBase : MonoBehaviour
{
    [SerializeField] float damage = 0.51f;

    protected bool IsSlashing { get; private set; }

    internal void DamageStart()
    { IsSlashing = true; }

    internal void DamageEnd()
    { IsSlashing = false; }

    protected void PerformDamage(IDamagereceiver damageReceiver)
    {
        damageReceiver?.ReceiveDamage(damage);
    }
}
