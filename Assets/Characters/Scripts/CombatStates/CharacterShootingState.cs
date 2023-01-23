using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterShootingState : CharacterStateBase
{
    // TODO: fix bug where changing weapon while aiming changes current weapon in CharacterEngine and disallows me to stop aiming.

    private bool aim = false;
    public bool shoot = false;

    private static Weapon currentWeapon;

    [HideInInspector] public UnityEvent<bool> onAim;
    [HideInInspector] public UnityEvent onShoot;

    private void Awake()
    {
        this.enabled = false;
    }

    [SerializeField] Transform bowstring;
    private Vector3 initialBowstringPosition;
    private void OnEnable()
    {
        onCombatStateEnteringOrExiting.Invoke(this);

        aim = true;
        onAim.Invoke(true);
        initialBowstringPosition = bowstring.localPosition;

        combatStateSpeedModifier = speed;
    }

    public override void ExitState()
    {
        StartCoroutine(OnExitState());
    }

    [SerializeField] GameObject loadedArrow;
    private IEnumerator OnExitState()
    {
        aim = false;
        onAim.Invoke(false);
        combatStateSpeedModifier = 0f;

        if (loadedArrow.activeInHierarchy)
        {
            loadedArrow.SetActive(false);
        }

        yield return new WaitUntil(() => aimingRig.weight <= 0f);

        aimingRig = null;
        onCombatStateEnteringOrExiting.Invoke(null);
    }

    public static void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        aimingRig = currentWeapon.AimingRig;
    }

    private void Update()
    {
        UpdateAim();

        UpdateShoot();

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
            if (!currentWeapon.CompareTag("Bow"))
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