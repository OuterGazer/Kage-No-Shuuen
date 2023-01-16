using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage = 0.51f;

    protected virtual void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
        Destroy(gameObject);
    }
}
