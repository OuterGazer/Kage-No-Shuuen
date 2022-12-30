using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableCube : MonoBehaviour, IDamagereceiver
{
    public void ReceiveDamage(float damage)
    {
        Destroy(gameObject);
    }
}
