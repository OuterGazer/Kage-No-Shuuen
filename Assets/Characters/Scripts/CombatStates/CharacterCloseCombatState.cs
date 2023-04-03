using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CharacterCloseCombatState : CharacterStateBase
{
    [SerializeField] float stepForwardLength = 1f;
    [SerializeField] float pushLengthOnEnemyTouch = 0.5f;

    [Header("SFX Clips")]
    [SerializeField] AudioClip katanaWoosh;
    [SerializeField] AudioClip armWoosh;

    private AudioSource audioSource;
    private static Weapon currentWeapon;
    public static void SetCurrentWeapon(Weapon inCurrentWeapon)
    {
        currentWeapon = inCurrentWeapon;
    }

    private bool slash = false;
    private bool heavySlash = false;
    private bool shouldStepForward = false;

    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;

    private void Awake()
    {

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateSlash();

        if (shouldStepForward) 
        { 
            if(currentWeapon.name.Contains("Katana") || currentWeapon.name.Contains("Nodachi")) { audioSource.PlayOneShot(katanaWoosh); }
            else if(currentWeapon.name.Contains("Gauntlet") || currentWeapon.name.Contains("Bomb")) { audioSource.PlayOneShot(armWoosh); }

            PushCharacterForward(stepForwardLength); 
        }
    }

    private void UpdateSlash()
    {
        if (slash)
        {
            onSlash.Invoke();
            slash = false;
        }
        else if (heavySlash)
        {
            onHeavySlash.Invoke();
            heavySlash = false;
        }
    }

    private NavMeshAgent enemy;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (this.enabled)
        {
            if (hit.collider.CompareTag("Soldier"))
            {
                if(!enemy)
                    enemy = hit.gameObject.GetComponentInParent<NavMeshAgent>();

                enemy?.Move(pushLengthOnEnemyTouch * transform.forward);
            }
        }
        else
        {
            enemy = null;
        }
    }

    // Methods called from animation events
    private void OnSlash() { slash = true; }
    private void OnHeavySlash() { heavySlash = true; }
    private void DamageStart() { shouldStepForward = true; }
    private void DamageEnd() { shouldStepForward = false; }
}
