using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArm : MonoBehaviour
{
    [Header("ShootData")]
    [SerializeField] BarrelBase[] barrel;    

    [Header("Debug")]
    [SerializeField] bool debugShot;
    [SerializeField] bool debugReload;

    [Header("ContinuousShooting")]
    [SerializeField] bool continuousShooting;
    [SerializeField] float maxShootsPerSecond;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (debugShot)
        {
            debugShot = false;
            Shoot();
        }

        if (debugReload)
        {
            debugReload = false;
            Reload();
        }
    }
#endif

    private void Update()
    {
#if UNITY_EDITOR
        UpdateDebug();
#endif
    }

#if UNITY_EDITOR
    float timeForNextShot = 0f;
    private void UpdateDebug()
    {
        if (continuousShooting)
        {
            timeForNextShot -= Time.deltaTime;
            if(timeForNextShot < 0f)
            {
                timeForNextShot += 1f / maxShootsPerSecond;
                Shoot();
            }
        }
    }
#endif

    public void Shoot()
    {
        foreach(BarrelBase b in barrel)
        {
            b.Shoot();
        }
        
    }

    public void Reload()
    {

    }

    // Aquí no tendremos código ni para apuntar o para crear dispersión de disparo, eso va en otras clases.
}
