using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBomb : ProjectileBase
{
    [Header("Explosion Settings")]
    [SerializeField] float explosionRange = 5f;

    [Header("VFX Settings")]
    [SerializeField] GameObject explosionVFX;

    protected override void OnTriggerEnter(Collider other)
    {
        // TODO: play SFX and VFX

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] damageableColliders = Physics.OverlapSphere(transform.position, explosionRange);

        foreach(Collider item in damageableColliders)
        {
            item.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
            RagdollController targetRagdoll = item.GetComponentInChildren<RagdollController>();
            targetRagdoll?.ActivateRagdoll();
            targetRagdoll?.ApplyExplosionForceToRagdoll(transform.position);
        }

        Destroy(explosion, 1.5f);
        Destroy(gameObject, 1.6f);
    }
}
