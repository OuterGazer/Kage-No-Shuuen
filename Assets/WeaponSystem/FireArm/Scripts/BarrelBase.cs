using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    [SerializeField] protected Transform shootPoint;
    public abstract void Shoot();
}
