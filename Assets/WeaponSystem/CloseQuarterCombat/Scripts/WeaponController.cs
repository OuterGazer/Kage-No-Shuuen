using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public bool slash = false;
    public bool heavySlash = false;
    public bool aim = false;

    Weapon[] weapons;
    Weapon currentWeapon;

    CharacterAnimator characterAnimator; // TODO: preguntar a Kike si esto lo dejo así o aplico animator override a través de evento
    [HideInInspector] public UnityEvent onWeaponChange;
    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;
    [HideInInspector] public UnityEvent<bool> onAim;

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
    }

    private void Update()
    {
        UpdateShoot();
        UpdateSlash();
        UpdateAim();
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
    
    private void UpdateShoot()
    {
        if (shoot && aim)
        {
            currentWeapon.shootingWeapon?.Shoot();
            shoot= false;
        }
    }

    [SerializeField] float animAcc = 0.05f;
    [SerializeField] Rig aimingRig;
    private void UpdateAim()
    {
        if (aim)
        {
            aimingRig.weight = (aimingRig.weight >= 1f) ? 1f : (aimingRig.weight += animAcc * Time.deltaTime);
        }
        else
        {
            aimingRig.weight = (aimingRig.weight <= 0f) ? 0f : (aimingRig.weight -= animAcc * Time.deltaTime);
        }
    }

    internal void DamageStart()
    {
        currentWeapon.closeCombatWeaponBase?.DamageStart();
    }

    internal void DamageEnd()
    {
        currentWeapon.closeCombatWeaponBase?.DamageEnd();
    }

    
    private void OnPrevWeapon()
    {
        if (IsWeaponCurrentlyNotBeingChanged())
        {
            prevWeapon = true;
            onWeaponChange.Invoke();
        }
    }

    private void OnNextWeapon()
    {
        if (IsWeaponCurrentlyNotBeingChanged())
        {
            nextWeapon = true;
            onWeaponChange.Invoke();
        }
    }
    private bool IsWeaponCurrentlyNotBeingChanged()
    {
        return !prevWeapon && !nextWeapon;
    }

    
    private void OnShoot() { shoot = true; }
    private void OnSlash() { slash = true; }
    private void OnHeavySlash() { heavySlash = true; }
    private void OnAim() { aim = !aim; onAim.Invoke(aim); }
}
