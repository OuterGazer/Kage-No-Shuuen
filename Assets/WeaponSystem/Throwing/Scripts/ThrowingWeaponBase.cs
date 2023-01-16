using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowingWeaponBase : MonoBehaviour
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float throwingStrength = 5f;
    [SerializeField] protected float spinSpeed = 10f;
    [SerializeField] protected Transform hand;
    [SerializeField] protected Transform player;

    protected Rigidbody projectileRB;

    public abstract void Throw();
}
