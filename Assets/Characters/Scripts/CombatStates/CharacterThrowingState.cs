using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterThrowingState : CharacterStateBase
{
    private bool throwing = false;

    private static Weapon currentWeapon;

    [HideInInspector] public UnityEvent onThrowing;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        onCombatStateEnteringOrExiting.Invoke(this);

        throwing = true;
    }

    public static void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void Update()
    {
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

    // Called from animation event
    public void ExitThrowingState()
    {
        onCombatStateEnteringOrExiting.Invoke(null);
    }
}
