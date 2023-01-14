using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalThrow : ThrowingWeaponBase
{
    [SerializeField] float range = 10f;
    [SerializeField] float spinSpeed = 10f;

    private Vector3 throwPosition= Vector3.zero;

    // Called from animation event
    public override void Throw()
    {
        GameObject shotProjectile = Instantiate(projectilePrefab, hand.position, player.rotation);

        projectileRB = shotProjectile.GetComponent<Rigidbody>();

        if (projectileRB)
        { 
            projectileRB.velocity = shotProjectile.transform.forward * throwingStrength;
            projectileRB.maxAngularVelocity = float.PositiveInfinity;
            projectileRB.AddRelativeTorque(shotProjectile.transform.up * spinSpeed, ForceMode.Impulse);
        }

        throwPosition = transform.position;
    }

    private void Update()
    {
        if (projectileRB)
        {
            Vector3 currentPosition = projectileRB.transform.position;

            if (HasProjectileReachedMaxRange(currentPosition))
                projectileRB.useGravity = true;
        }
    }

    private bool HasProjectileReachedMaxRange(Vector3 currentPosition)
    {
        return (currentPosition - throwPosition).sqrMagnitude >= range * range;
    }
}
