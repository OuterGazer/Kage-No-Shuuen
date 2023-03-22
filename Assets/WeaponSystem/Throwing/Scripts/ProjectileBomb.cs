using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileBomb : ProjectileBase
{
    [Header("Explosion Settings")]
    [SerializeField] float explosionRange = 5f;
    [SerializeField] LayerMask objectsAffectedByExplosions = Physics.DefaultRaycastLayers;

    [Header("VFX Settings")]
    [SerializeField] GameObject explosionVFX;

    protected override void OnTriggerEnter(Collider other)
    {
        // TODO: play SFX

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] damageableColliders = Physics.OverlapSphere(transform.position, explosionRange, objectsAffectedByExplosions);

        foreach(Collider item in damageableColliders)
        {
            item.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
            
            RagdollController targetRagdoll = null;
            if (item.CompareTag("Player"))
            {
                targetRagdoll = item.GetComponentInChildren<RagdollController>();
            }
            else if (item.CompareTag("Soldier"))
            {
                targetRagdoll = item.GetComponentInParent<NavMeshAgent>()?.GetComponentInChildren<RagdollController>(); // Only happens one frame
            }

            targetRagdoll?.ActivateRagdoll();
            targetRagdoll?.ApplyExplosionForceToRagdoll(transform.position);
        }

        Destroy(explosion, 1.5f);
        Destroy(gameObject, 1.6f);
    }
}
