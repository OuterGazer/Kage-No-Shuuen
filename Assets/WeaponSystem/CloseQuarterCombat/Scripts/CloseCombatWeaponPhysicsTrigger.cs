using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloseCombatWeaponPhysicsTrigger : CloseCombatWeaponBase
{
    [SerializeField] GameObject bomb;

    [SerializeField] AudioClip hitSound;

    private AudioSource audiosource;
    private bool hasSoundPlayed = false;

    private bool hasSpawnedBomb = false;

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        hasSpawnedBomb = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(tag) && IsSlashing)
        {
            IDamagereceiver damageReceiver = other.GetComponent<IDamagereceiver>();
            PerformDamage(damageReceiver);
            if (CompareTag("Player")) { PlayHitsound(); }

            if (name.Contains("Bomb") && !hasSpawnedBomb)
            {
                GameObject bombInstance = Instantiate(bomb, transform.position, Quaternion.identity);
                bombInstance.GetComponentInChildren<SphereCollider>().radius = 5f;
                hasSpawnedBomb = true;
            }
        }
    }

    private void PlayHitsound()
    {
        if (!hasSoundPlayed)
        {
            audiosource.PlayOneShot(hitSound);
            StartCoroutine(ResetSound());
        }
    }

    private IEnumerator ResetSound()
    {
        yield return new WaitForSeconds(1f);
        hasSoundPlayed = false;
    }
}
