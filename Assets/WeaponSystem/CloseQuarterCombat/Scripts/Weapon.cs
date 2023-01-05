using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AnimatorOverrideController animatorOverride;
    public ShootingWeapon shootingWeapon;
    public CloseCombatWeaponBase closeCombatWeaponBase;

    private void Reset()
    {
        shootingWeapon = GetComponent<ShootingWeapon>();
        closeCombatWeaponBase= GetComponent<CloseCombatWeaponBase>();
    }
}
