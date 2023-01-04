using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AnimatorOverrideController animatorOverride;
    public FireArm fireArm;
    public CloseCombatWeaponBase closeCombatWeaponBase;

    private void Reset()
    {
        fireArm = GetComponent<FireArm>();
        closeCombatWeaponBase= GetComponent<CloseCombatWeaponBase>();
    }
}
