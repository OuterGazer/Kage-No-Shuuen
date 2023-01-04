using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] Transform weaponsParent;
    int currentWeaponIndex;

    public bool prevWeapon;
    public bool nextWeapon;
    public bool shoot;
    public bool slash;

    Weapon[] weapons;
    Weapon currentWeapon;

    CharacterAnimator characterAnimator;
    [HideInInspector] public UnityEvent onWeaponChange;

    private void Awake()
    {
        weapons = weaponsParent.GetComponentsInChildren<Weapon>();
        characterAnimator= GetComponentInChildren<CharacterAnimator>();
    }

    private void Start()
    {
        for(int i = 1; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }

        // Es mucho asumir que tendremos un arma inicial, pero peor es robar.
        // Se podr�a hacer un check para saber que weapons.Length no sea cero.
        currentWeapon = weapons[0];
        // TODO: esta l�nea aplica el animator controller propio de cada arma cuando implementemos el animatorcontrolleroverride
        //characterAnimator.ApplyAnimatorController(weapons[0]);
    }

    private void Update()
    {
        UpdateShoot();
        UpdateSlash();
    }    

    // Gets called from an animation event
    private void ChangeCurrentWeapon()
    {
        if (prevWeapon)
        {
            //prevWeapon = false;
            SelectWeaponInDirection(-1);
        }

        if (nextWeapon)
        {
            //nextWeapon = false;
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
        // TODO: esta l�nea aplica el animator controller propio de cada arma cuando implementemos el animatorcontrolleroverride
        // characterAnimator.ApplyAnimatorController(weapons[currentWeaponIndex]);
    }

    private void ResetWeaponChangeState()
    {
        prevWeapon = false;
        nextWeapon = false;
    }

    private void UpdateShoot()
    {
        if (shoot)
        {
            // Aqui falta decirle al animator que active la animacion de disparo.
            // Tambi�n falta poner un m�todo que active la animaci�n de apuntado con el resto de eventos en la parte baja del script.
            // Ahora mismo se dispara sin apuntar primero y sale el raycast al suelo, debo a�adir un evento de animaci�n para que se dispare en el momento apropiado.
            currentWeapon.fireArm?.Shoot();
            shoot= false;
        }
    }

    private void UpdateSlash()
    {
        if (slash)
        {
            // TODO: esta l�nea es para cuando implmentemos animaci�n de ataque
            //characterAnimator.PerformSlash();
            slash = false;
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
}
