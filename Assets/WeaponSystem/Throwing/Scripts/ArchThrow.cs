using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchThrow : ThrowingWeaponBase
{
    // Called from animation event
    public override void Throw()
    {
        GameObject shotProjectile = Instantiate(projectilePrefab, hand.position, player.rotation);

        projectileRB = shotProjectile.GetComponent<Rigidbody>();

        if (projectileRB)
        {
            //projectileRB.velocity = shotProjectile.transform.forward * throwingStrength;
            projectileRB.AddForce(shotProjectile.transform.forward * throwingStrength, ForceMode.Impulse);
        }
    }
}
