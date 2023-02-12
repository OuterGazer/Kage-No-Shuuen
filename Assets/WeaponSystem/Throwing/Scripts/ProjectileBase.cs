using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage = 0.51f;

    private string ownerTag;

    public void SetOwnerTag(string inTag)
    {
        ownerTag = inTag;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!gameObject.CompareTag(ownerTag))
        {
            other.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
            Destroy(gameObject);
        }
    }
}
