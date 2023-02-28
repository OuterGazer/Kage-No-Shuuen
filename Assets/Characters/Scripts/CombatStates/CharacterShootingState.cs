using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterShootingState : CharacterStateBase
{
    private bool aim = false;
    public bool shoot = false;

    private static Weapon currentWeapon;

    [HideInInspector] public UnityEvent<bool> onAim;
    [HideInInspector] public UnityEvent onShoot;

    [SerializeField] Transform bowstring;
    private Vector3 initialBowstringPosition;
    private void OnEnable()
    {
        aim = true;
        onAim.Invoke(true);
        initialBowstringPosition = bowstring.localPosition;
    }

    [SerializeField] GameObject loadedArrow;
    private void OnDisable()
    {
        aim = false;
        onAim.Invoke(false);

        if (loadedArrow.activeInHierarchy)
        {
            loadedArrow.SetActive(false);
        }
        
        if (aimingRig)
            { aimingRig.weight = 0f; }

        bowstring.localPosition = initialBowstringPosition;
        isPullingBowstring = false;
        aimingRig = null;
    }

    
    //public override void ExitState()
    //{
        

    //    StartCoroutine(OnExitState());
    //}

    // Method necessary to make the unaim animation look better
    //private IEnumerator OnExitState()
    //{
    //    aim = false;
    //    onAim.Invoke(false);
    //    combatStateSpeedModifier = 0f;

    //    if (loadedArrow.activeInHierarchy)
    //    {
    //        loadedArrow.SetActive(false);
    //    }

    //    yield return new WaitUntil(() => aimingRig.weight <= 0f);

    //    aimingRig = null;
    //}

    public static void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        aimingRig = currentWeapon.AimingRig;
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

        UpdateAim();

        UpdateShoot();
    }

    private void LateUpdate()
    {
        OrientateCharacterForward();
    }

    [SerializeField] float animAcc = 0.05f;
    private static Rig aimingRig;
    [SerializeField] Transform leftHand;
    private void UpdateAim()
    {
        if (aim)
        {
            aimingRig.weight = (aimingRig.weight >= 1f) ? 1f : (aimingRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim)
        {
            aimingRig.weight = (aimingRig.weight <= 0f) ? 0f : (aimingRig.weight -= animAcc * Time.deltaTime);

            isPullingBowstring = false;
            bowstring.localPosition = initialBowstringPosition;
        }

        if (isPullingBowstring)
        {
            bowstring.localPosition += bowstring.InverseTransformPoint(leftHand.TransformPoint(leftHand.localPosition));
        }
        else if(!isPullingBowstring)
        {
            bowstring.localPosition = Vector3.MoveTowards(bowstring.localPosition, initialBowstringPosition, 20f * Time.deltaTime);
        }
    }

    private void UpdateShoot()
    {
        if (shoot && aim)
        {
            if (!currentWeapon.name.Equals("Bow"))
            { currentWeapon.ShootingWeapon?.Shoot(); }

            onShoot.Invoke();
            shoot = false;
        }
    }

    // Called from Animation Event
    [SerializeField] GameObject quiverArrow;
    internal void SpawnArrowInHand()
    {
        quiverArrow.SetActive(true);
    }

    // Called from Animation Event
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

    // Called from an animation event
    internal void ShootBow()
    {
        currentWeapon.ShootingWeapon?.Shoot();
    }

    // Called from Animation Event
    internal void ReloadBow()
    {
        onAim.Invoke(true);
        initialBowstringPosition = bowstring.localPosition;
    }

    private void OnShoot() 
    {
        if (this.enabled)
        { shoot = true; }
    }
}
