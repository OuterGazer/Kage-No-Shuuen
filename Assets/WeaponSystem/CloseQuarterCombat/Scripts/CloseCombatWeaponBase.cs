using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseCombatWeaponBase : MonoBehaviour
{
    [SerializeField] float damage = 0.51f;
    public float Damage => damage;
    protected string ownerTag;

    protected bool IsSlashing { get; private set; }

    // Called from an animation event
    internal void DamageStart()
    { IsSlashing = true; }

    // Called from an animation event
    internal void DamageEnd()
    { IsSlashing = false; }

    private void OnEnable()
    {
        ownerTag = GetComponentInParent<WeaponController>().tag;
        gameObject.tag = ownerTag;
    }

    protected void PerformDamage(IDamagereceiver damageReceiver)
    {
        damageReceiver?.ReceiveDamage(damage);
    }
}
