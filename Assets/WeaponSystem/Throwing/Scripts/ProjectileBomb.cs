using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBomb : ProjectileBase
{
    [SerializeField] float explosionRange = 5f;

    protected override void OnTriggerEnter(Collider other)
    {
        // TODO: play SFX and VFX

        Collider[] damageableColliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach(Collider item in damageableColliders)
        {
            item.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage); 
        }

        Destroy(gameObject);
    }
}
