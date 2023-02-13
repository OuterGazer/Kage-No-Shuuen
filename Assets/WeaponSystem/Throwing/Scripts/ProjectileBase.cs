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
        gameObject.tag = ownerTag;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ownerTag))
        {
            other.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
            Destroy(gameObject);
        }
    }
}
