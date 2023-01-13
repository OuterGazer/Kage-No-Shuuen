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
    public bool slash = false;
    public bool heavySlash = false;
    public bool aim = false;
    public bool throwing = false;

    Weapon[] weapons;
    Weapon currentWeapon;

    CharacterAnimator characterAnimator; // TODO: preguntar a Kike si esto lo dejo así o aplico animator override a través de evento
    [HideInInspector] public UnityEvent onWeaponChange;
    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;
    [HideInInspector] public UnityEvent<bool> onAim;
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
    }

    private void Update()
    {
        UpdateShoot();
        UpdateSlash();
        UpdateAim();
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
            if (currentWeapon.CompareTag("Fukiya") || currentWeapon.CompareTag("Gauntlet"))
            {
                currentWeapon.ShootingWeapon?.Shoot();                
            }
            else if (currentWeapon.CompareTag("Bow"))
            {
                bowstring.localPosition = Vector3.MoveTowards(bowstring.localPosition, initialBowstringPosition, 20f * Time.deltaTime);
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
        aim = false;
    }

    // TODO: refactor this in its own state classes
    [SerializeField] float animAcc = 0.05f;
    [SerializeField] Rig aimingFukiyaRig;
    [SerializeField] Rig aimingBowRig;
    [SerializeField] Rig aimingGauntletRig;
    [SerializeField] Transform bowstring;
    [SerializeField] Transform leftHand;
    [SerializeField] Vector3 initialBowstringPosition;
    private void UpdateAim()
    {
        if (aim && currentWeapon.CompareTag("Fukiya"))
        {
            aimingFukiyaRig.weight = (aimingFukiyaRig.weight >= 1f) ? 1f : (aimingFukiyaRig.weight += animAcc * Time.deltaTime);
        }
        else if(!aim && currentWeapon.CompareTag("Fukiya"))
        {
            aimingFukiyaRig.weight = (aimingFukiyaRig.weight <= 0f) ? 0f : (aimingFukiyaRig.weight -= animAcc * Time.deltaTime);
        }

        if (aim && currentWeapon.CompareTag("Bow"))
        {
            
            aimingBowRig.weight = (aimingBowRig.weight >= 1f) ? 1f : (aimingBowRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim && currentWeapon.CompareTag("Bow"))
        {
            aimingBowRig.weight = (aimingBowRig.weight <= 0f) ? 0f : (aimingBowRig.weight -= animAcc * Time.deltaTime);
            isPullingBowstring = false;
            bowstring.localPosition = initialBowstringPosition;
        }

        if (isPullingBowstring)
        {
            bowstring.localPosition += bowstring.InverseTransformPoint(leftHand.TransformPoint(leftHand.localPosition));
        }
        else
        {
            bowstring.localPosition = Vector3.MoveTowards(bowstring.localPosition, initialBowstringPosition, 20f * Time.deltaTime);
        }

        if (aim && currentWeapon.CompareTag("Gauntlet"))
        {
            aimingGauntletRig.weight = (aimingGauntletRig.weight >= 1f) ? 1f : (aimingGauntletRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim && currentWeapon.CompareTag("Gauntlet"))
        {
            aimingGauntletRig.weight = (aimingGauntletRig.weight <= 0f) ? 0f : (aimingGauntletRig.weight -= animAcc * Time.deltaTime);
        }
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

    // Called from Animation Event
    [SerializeField] GameObject quiverArrow;
    internal void SpawnArrowInHand()
    {
        quiverArrow.SetActive(true);
    }

    // Called from Animation Event
    [SerializeField] GameObject loadedArrow;
    internal void SpawnArrowInBow()
    {
        quiverArrow.SetActive(false);
        loadedArrow.SetActive(true);
    }

    // Called from Animation Event
    private bool isPullingBowstring = false;
    internal void PullBowstring()
    {
        isPullingBowstring = true;
    }
    // Called from Animation Event
    internal void ReleaseBowstring()
    {
        isPullingBowstring = false;
        loadedArrow.SetActive(false);
    }

    private void UpdateThrow()
    {
        if (throwing)
        {
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
    private void OnAim() { aim = !aim; onAim.Invoke(aim); if (loadedArrow.activeInHierarchy) { loadedArrow.SetActive(false); } }
    private void OnWeaponThrow() { throwing = true; }
}
