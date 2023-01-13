using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController animatorOverride;
    public AnimatorOverrideController AnimatorOverride => animatorOverride;
    [SerializeField] ShootingWeapon shootingWeapon;
    public ShootingWeapon ShootingWeapon => shootingWeapon;
    [SerializeField] CloseCombatWeaponBase closeCombatWeaponBase;
    public CloseCombatWeaponBase CloseCombatWeaponBase => closeCombatWeaponBase;
    [SerializeField] ThrowingWeaponBase throwingWeaponBase;
    public ThrowingWeaponBase ThrowingWeaponBase => throwingWeaponBase;

    private void Reset()
    {
        shootingWeapon = GetComponent<ShootingWeapon>();
        closeCombatWeaponBase= GetComponent<CloseCombatWeaponBase>();
        throwingWeaponBase= GetComponent<ThrowingWeaponBase>();
    }
}
