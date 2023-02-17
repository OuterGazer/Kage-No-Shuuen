using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInstantiate : BarrelBase
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float launchSpeed = 5f;

    // Como siempre hacemos que el forward de todo vaya hacia adelante, usamos shootPoint.rotation y eso nos asegura 100% que esté alineado correctamente y no tenemos que usar Quaternion.identity
    public override void Shoot()
    {
        GameObject shotProjectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        shotProjectile.GetComponent<ProjectileBase>().SetOwnerTag(transform.parent.tag);

        Rigidbody projectileRB = shotProjectile.GetComponent<Rigidbody>();

        if (projectileRB)
        { projectileRB.velocity = shotProjectile.transform.forward* launchSpeed; }
            
    }
}
