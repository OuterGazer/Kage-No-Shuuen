using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Transform weaponsParent;
    int currentWeaponIndex;
    [SerializeField] AudioClip unsheath;

    private bool prevWeapon = false;
    private bool nextWeapon = false;
    private bool aim = false;
    public void SetAim(bool aim)
    {
        this.aim = aim;
    }

    List<Weapon> weapons = new();
    Weapon currentWeapon;

    private AudioSource audioSource;
    CharacterAnimator characterAnimator; // TODO: preguntar a Kike si esto lo dejo así o aplico animator override a través de evento
    [HideInInspector] public UnityEvent<Weapon> onWeaponChange;
    public UnityEvent<int> OnWeaponIndexChange;

    private void Awake()
    {
        weapons = weaponsParent.GetComponentsInChildren<Weapon>().ToList<Weapon>();
        characterAnimator= GetComponent<CharacterAnimator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        foreach(Weapon item in weapons)
        {
            item.ownerTag = tag;
            item.tag = item.ownerTag;
            Collider temp = item.GetComponentInChildren<Collider>();
            if(temp)
                temp.tag = item.ownerTag;
        }
    }

    private void Start()
    {
        for(int i = 1; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }

        // Es mucho asumir que tendremos un arma inicial, pero peor es robar.
        // Se podría hacer un check para saber que weapons.Length no sea cero.
        currentWeapon = weapons[0];
        characterAnimator.ApplyAnimatorController(currentWeapon);
        onWeaponChange.Invoke(currentWeapon);
        OnWeaponIndexChange.Invoke(0);
    }

    public void AddWeapon(Weapon weapon)
    {
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);
            weapon.ownerTag = tag;
            weapon.tag = weapon.ownerTag;
            Collider temp = weapon.GetComponentInChildren<Collider>();
            if (temp)
                temp.tag = weapon.ownerTag;
        }
        
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

        if(currentWeapon.name.Contains("Katana") || currentWeapon.name.Contains("Nodachi")) { audioSource.PlayOneShot(unsheath); }
        onWeaponChange.Invoke(currentWeapon);
        OnWeaponIndexChange.Invoke(currentWeaponIndex);
    }

    private void SelectWeaponInDirection(int direction) // +1 -1
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false);
        currentWeaponIndex += direction;

        if (currentWeaponIndex < 0)
        { currentWeaponIndex = weapons.Count - 1; }
        if (currentWeaponIndex >= weapons.Count)
        { currentWeaponIndex = 0; }

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        weapons[currentWeaponIndex].gameObject.SetActive(true);        
    }

    // Called from an animation event
    private void ResetWeaponChangeState()
    {
        prevWeapon = false;
        nextWeapon = false;

        // TODO: have ApplyAnimatorController() to be called by an avent here and pplied internally in CharacterAnimator
        characterAnimator.ApplyAnimatorController(weapons[currentWeaponIndex]);
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
    
    private void OnPrevWeapon()
    {
        if (!aim)
        {
            if (IsWeaponCurrentlyNotBeingChanged())
            {
                prevWeapon = true;
                onWeaponChange.Invoke(null);
            }
        }
    }

    private void OnNextWeapon()
    {
        if (!aim)
        {
            if (IsWeaponCurrentlyNotBeingChanged())
            {
                nextWeapon = true;
                onWeaponChange.Invoke(null);
            }
        }
    }
    private bool IsWeaponCurrentlyNotBeingChanged()
    {
        return !prevWeapon && !nextWeapon;
    }

    // Event from PlayerInput
    private void OnAim()
    {
        if (currentWeapon?.AimingRig) { aim = !aim; }
    }
}
