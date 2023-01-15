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

    public bool IsThrowing { get; private set; }

    public void StartThrowing() { IsThrowing = true; }
    public void EndThrowing() { IsThrowing = false; }

    public abstract void Throw();
}
