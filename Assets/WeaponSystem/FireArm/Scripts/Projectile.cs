using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage = 50f;

    private void OnCollisionEnter(Collision collision)
    {
        collision.collider.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
        Destroy(gameObject);
    }
}
