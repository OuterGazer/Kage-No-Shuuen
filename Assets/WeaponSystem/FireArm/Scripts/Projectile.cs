using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage = 0.51f;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
        Destroy(gameObject);
    }
}
