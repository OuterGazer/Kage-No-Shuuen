using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Transform weaponsParent;
    int currentWeaponIndex;

    public bool prevWeapon = false;
    public bool nextWeapon = false;
    public bool shoot = false;
    
    
    public bool throwing = false;

    Weapon[] weapons;
    Weapon currentWeapon;

    CharacterAnimator characterAnimator; // TODO: preguntar a Kike si esto lo dejo así o aplico animator override a través de evento
    [HideInInspector] public UnityEvent<Weapon> onWeaponChange;
    [HideInInspector] public UnityEvent onAimingShootingWeapon;
    
    
    [HideInInspector] public UnityEvent onShoot;
    [HideInInspector] public UnityEvent onThrowing;

    private void Awake()
    {
        weapons = weaponsParent.GetComponentsInChildren<Weapon>();
        characterAnimator= GetComponent<CharacterAnimator>();
    }

    private void Start()
    {
        for(int i = 1; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }

        // Es mucho asumir que tendremos un arma inicial, pero peor es robar.
        // Se podría hacer un check para saber que weapons.Length no sea cero.
        currentWeapon = weapons[0];
        characterAnimator.ApplyAnimatorController(weapons[0]);
        onWeaponChange.Invoke(currentWeapon);
    }

    private void Update()
    {
        UpdateShoot();
        
        
        UpdateThrow();
    }    

    // Gets called from an animation event
    private void ChangeCurrentWeapon()
    {
        if (prevWeapon)
        {
            SelectWeaponInDirection(-1);
        }

        if (nextWeapon)
        {
            SelectWeaponInDirection(+1);
        }

        onWeaponChange.Invoke(currentWeapon);
    }

    private void SelectWeaponInDirection(int direction) // +1 -1
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex += direction;

        if (currentWeaponIndex < 0)
        { currentWeaponIndex = weapons.Length - 1; }
        if (currentWeaponIndex >= weapons.Length)
        { currentWeaponIndex = 0; }

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        weapons[currentWeaponIndex].gameObject.SetActive(true);        
    }

    // Called from an animation event
    private void ResetWeaponChangeState()
    {
        prevWeapon = false;
        nextWeapon = false;

        characterAnimator.ApplyAnimatorController(weapons[currentWeaponIndex]);
    }
    
    private void UpdateShoot()
    {
        if (shoot)// && aim)
        {
            if (currentWeapon.CompareTag("Fukiya") || currentWeapon.CompareTag("Gauntlet"))
            {
                currentWeapon.ShootingWeapon?.Shoot();                
            }
            else if (currentWeapon.CompareTag("Bow"))
            {
                //bowstring.localPosition = Vector3.MoveTowards(bowstring.localPosition, initialBowstringPosition, 20f * Time.deltaTime);
            }               

            onShoot.Invoke();
            shoot = false;
        }
    }

    // Called from an animation event
    internal void ShootBow()
    {
        currentWeapon.ShootingWeapon?.Shoot();
    }

    // Called from Animation Event
    internal void ExitShooting()
    {
        //aim = false;
    }

    // Called from Animation Event
    internal void DamageStart()
    {
        currentWeapon.CloseCombatWeaponBase?.DamageStart();
    }

    // Called from Animation Event
    internal void DamageEnd()
    {
        currentWeapon.CloseCombatWeaponBase?.DamageEnd();
    }

    private void UpdateThrow()
    {
        if (throwing)
        {
            // TODO: pasar un parámetro para determinar la velocidad de animación en CharacterAnimator
            onThrowing.Invoke();
            throwing = false;
        }
    }

    // Called from aniamtion event
    private void ThrowWeapon()
    {
        currentWeapon.ThrowingWeaponBase?.Throw();
    }

    
    private void OnPrevWeapon()
    {
        if (IsWeaponCurrentlyNotBeingChanged())
        {
            prevWeapon = true;
            onWeaponChange.Invoke(null);
        }
    }

    private void OnNextWeapon()
    {
        if (IsWeaponCurrentlyNotBeingChanged())
        {
            nextWeapon = true;
            onWeaponChange.Invoke(null);
        }
    }
    private bool IsWeaponCurrentlyNotBeingChanged()
    {
        return !prevWeapon && !nextWeapon;
    }
    
    private void OnShoot() { shoot = true; }
    
    
    private void OnWeaponThrow() { throwing = true; }
}
