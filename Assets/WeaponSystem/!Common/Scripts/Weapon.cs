using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
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

    [SerializeField] Rig aimingRig;
    public Rig AimingRig => aimingRig;

    public string ownerTag;

    private void Reset()
    {
        shootingWeapon = GetComponent<ShootingWeapon>();
        closeCombatWeaponBase= GetComponent<CloseCombatWeaponBase>();
        throwingWeaponBase= GetComponent<ThrowingWeaponBase>();
    }
}
