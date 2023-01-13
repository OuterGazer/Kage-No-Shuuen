using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalThrow : ThrowingWeaponBase
{
    [SerializeField] float range = 10f;
    [SerializeField] Vector3 hand;

    // Called from animation event
    public override void ThrowWeapon()
    {
        GameObject shotProjectile = Instantiate(projectilePrefab, hand, Quaternion.identity);

        Rigidbody projectileRB = shotProjectile.GetComponent<Rigidbody>();

        if (projectileRB)
        { projectileRB.velocity = shotProjectile.transform.forward * throwingStrength; }

        //TODO: use range to activate gravity in the projectile's rigidbody to have it fall down
    }
}
