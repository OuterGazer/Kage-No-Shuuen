using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterAimingState : CharacterStateBase
{
    private bool aim = false;

    [HideInInspector] public UnityEvent<bool> onAim;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        onCombatStateEnteringOrExiting.Invoke(this);

        aim = true;
        onAim.Invoke(true);

        combatStateSpeedModifier = speed;
    }

    public override void ExitState()
    {
        StartCoroutine(OnExitState());
    }

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

    public static void SetAimingRig(Rig inAimingRig)
    {
        aimingRig = inAimingRig;
    }

    private void Update()
    {
        UpdateAim();

        OrientateCharacterForward();
    }

    [SerializeField] float animAcc = 0.05f;
    private static Rig aimingRig;
    [SerializeField] Transform bowstring;
    [SerializeField] Transform leftHand;
    [SerializeField] Vector3 initialBowstringPosition;
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
        else
        {
            bowstring.localPosition = Vector3.MoveTowards(bowstring.localPosition, initialBowstringPosition, 20f * Time.deltaTime);
        }
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
}
