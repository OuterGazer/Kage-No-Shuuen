using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterBlockingState : CharacterStateBase
{
    [SerializeField] Rig blockingRig;
    private bool isBlocking = false;

    [HideInInspector] public UnityEvent<bool> UpdateBlockingStatus; // Event for the animator

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        onCombatStateEnteringOrExiting.Invoke(this);

        UpdateBlockingStatus.Invoke(true);
        isBlocking = true;

        combatStateSpeedModifier = speed;
    }

    private void Update()
    {
        ChangeBlockingRiggingWeight();
    }

    [SerializeField] float weightChangeAcceleration = 1f;
    private void ChangeBlockingRiggingWeight()
    {
        if (!isBlocking)
        {
            if (blockingRig.weight >= 0f)
                blockingRig.weight -= weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 0f;
        }
        else
        {
            if (blockingRig.weight <= 1f)
                blockingRig.weight += weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 1f;
        }
    }

    public override void ExitState()
    {
        StartCoroutine(OnExitState());
    }

    private IEnumerator OnExitState()
    {
        UpdateBlockingStatus.Invoke(false);
        isBlocking = false;
        combatStateSpeedModifier = 0f;

        yield return new WaitUntil(() => blockingRig.weight <= 0f);

        onCombatStateEnteringOrExiting.Invoke(null);
    }
}
