using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchThrow : ThrowingWeaponBase
{
    [SerializeField] float throwAngle;

    // Called from animation event
    public override void Throw()
    {
        GameObject shotProjectile = Instantiate(projectilePrefab, hand.position,  player.rotation);
        shotProjectile.transform.localRotation *=  Quaternion.Euler(-throwAngle, 0f, 0f);

        projectileRB = shotProjectile.GetComponent<Rigidbody>();

        if (projectileRB)
        {
            projectileRB.AddForce(shotProjectile.transform.forward * throwingStrength, ForceMode.Impulse);
            projectileRB.AddTorque(shotProjectile.transform.right * spinSpeed, ForceMode.Impulse);
        }
    }
}
