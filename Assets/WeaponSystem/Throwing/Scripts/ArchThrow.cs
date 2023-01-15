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
            // TODO: have arch projectiles come at an angle. Right now bomb and makibisi spawn perfectly horizontal and get pushed horizontally.
            projectileRB.AddForce(shotProjectile.transform.forward * throwingStrength, ForceMode.Impulse);
            projectileRB.AddTorque(shotProjectile.transform.right * spinSpeed, ForceMode.Impulse);
        }
    }
}
