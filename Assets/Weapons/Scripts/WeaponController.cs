using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        // Se podría hacer un check para saber que weapons.Length no sea cero.
        currentWeapon = weapons[0];
        // TODO: esta línea aplica el animator controller propio de cada arma cuando implementemos el animatorcontrolleroverride
        //characterAnimator.ApplyAnimatorController(weapons[0]);
    }

    private void Update()
    {
        UpdateCurrentWeapon();
        UpdateShoot();
        UpdateSlash();
    }    

    private void UpdateCurrentWeapon()
    {
        if (prevWeapon)
        {
            prevWeapon = false;
            SelectWeaponInDirection(-1);
        }

        if (nextWeapon)
        {
            nextWeapon = false;
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

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>(); // Esta linea puede que le falte a Kike o a lo mejor se olvidó mencionar que la escribió.
        weapons[currentWeaponIndex].gameObject.SetActive(true);
        // TODO: esta línea aplica el animator controller propio de cada arma cuando implementemos el animatorcontrolleroverride
        // characterAnimator.ApplyAnimatorController(weapons[currentWeaponIndex]);
    }

    private void UpdateShoot()
    {
        if (shoot)
        {
            // Aqui falta decirle al animator que active la animacion de disparo.
            // También falta poner un método que active la animación de apuntado con el resto de eventos en la parte baja del script.
            // Ahora mismo se dispara sin apuntar primero y sale el raycast al suelo, debo añadir un evento de animación para que se dispare en el momento apropiado.
            currentWeapon.fireArm?.Shoot();
            shoot= false;
        }
    }

    private void UpdateSlash()
    {
        if (slash)
        {
            // TODO: esta línea es para cuando implmentemos animación de ataque
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

    
    private void OnPrevWeapon() { prevWeapon = true; }
    private void OnNextWeapon() { nextWeapon= true; }
    private void OnShoot() { shoot = true; }
    private void OnSlash() { slash = true; }
}
