using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterThrowingState : CharacterStateBase
{
    [SerializeField] AudioClip lightingBombSound;

    private bool throwing = false;
    private bool hasSoundPlayed = false;

    private static Weapon currentWeapon;

    [HideInInspector] public UnityEvent onThrowing;

    private void OnEnable()
    {
        throwing = true;
    }

    private void OnDisable()
    {
        hasSoundPlayed = false;
    }

    public static void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void Update()
    {
        if (!hasSoundPlayed) { AudioSource.PlayClipAtPoint(lightingBombSound, transform.position); hasSoundPlayed = true; }
        UpdateThrow();
    }

    private void UpdateThrow()
    {
        if (throwing)
        {
            onThrowing.Invoke();
            throwing = false;
        }
    }

    // Called from animation event
    private void ThrowWeapon()
    {
        currentWeapon.ThrowingWeaponBase?.Throw();
    }
}
