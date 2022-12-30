using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInstaShot : BarrelBase
{
    [SerializeField] float maxDistance = 50.0f;
    [SerializeField] float damage = 1f;

    public override void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, maxDistance))
        {
            Debug.DrawLine(shootPoint.position, hit.point, Color.red, 10.0f);
            // Recomendable por expresividad
            //bool isHit = hit.collider.GetComponent<IDamagereceiver>() != null ? true : false;
            hit.collider.GetComponent<IDamagereceiver>()?.ReceiveDamage(damage);
        }
        else
        {
            Debug.DrawRay(shootPoint.position, shootPoint.forward * maxDistance, Color.blue, 1f);
        }
    }
}
