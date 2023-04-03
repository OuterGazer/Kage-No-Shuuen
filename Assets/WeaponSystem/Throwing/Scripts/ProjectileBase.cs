using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected float damage = 0.51f;
    [SerializeField] AudioClip hitSound;

    private string ownerTag;

    public void SetOwnerTag(string inTag)
    {
        ownerTag = inTag;
        gameObject.tag = ownerTag;
        gameObject.GetComponentInChildren<Collider>().tag = inTag;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ownerTag))
        {
            other.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            Destroy(gameObject);
        }
    }
}
