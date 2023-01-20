using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.Burst.Intrinsics;
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
    }

    public override void ExitState()
    {
        onCombatStateEnteringOrExiting.Invoke(this);
    }

    private void Update()
    {
        UpdateAim();
    }

    [SerializeField] float animAcc = 0.05f;
    [SerializeField] Rig aimingFukiyaRig;
    [SerializeField] Rig aimingBowRig;
    [SerializeField] Rig aimingGauntletRig;
    [SerializeField] Transform bowstring;
    [SerializeField] Transform leftHand;
    [SerializeField] Vector3 initialBowstringPosition;
    private void UpdateAim()
    {
        if (aim && this.CompareTag("Fukiya"))
        {
            aimingFukiyaRig.weight = (aimingFukiyaRig.weight >= 1f) ? 1f : (aimingFukiyaRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim && this.CompareTag("Fukiya"))
        {
            aimingFukiyaRig.weight = (aimingFukiyaRig.weight <= 0f) ? 0f : (aimingFukiyaRig.weight -= animAcc * Time.deltaTime);
        }

        if (aim && this.CompareTag("Bow"))
        {

            aimingBowRig.weight = (aimingBowRig.weight >= 1f) ? 1f : (aimingBowRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim && this.CompareTag("Bow"))
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

        if (aim && this.CompareTag("Gauntlet"))
        {
            aimingGauntletRig.weight = (aimingGauntletRig.weight >= 1f) ? 1f : (aimingGauntletRig.weight += animAcc * Time.deltaTime);
        }
        else if (!aim && this.CompareTag("Gauntlet"))
        {
            aimingGauntletRig.weight = (aimingGauntletRig.weight <= 0f) ? 0f : (aimingGauntletRig.weight -= animAcc * Time.deltaTime);
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

    private void OnAim() { aim = !aim; onAim.Invoke(aim); if (loadedArrow.activeInHierarchy) { loadedArrow.SetActive(false); } }
}
