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
        UpdateBlockingStatus.Invoke(true);
        isBlocking = true;
    }

    private void OnDisable()
    {
        UpdateBlockingStatus.Invoke(false);
        isBlocking = false;
        blockingRig.weight = 0;
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

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
        // StartCoroutine(OnExitState());
    }

    // Method necessary to make unblocking animation look better
    //private IEnumerator OnExitState() 
    //{
    //    UpdateBlockingStatus.Invoke(false);
    //    isBlocking = false;
    //    combatStateSpeedModifier = 0f;

    //    yield return new WaitUntil(() => blockingRig.weight <= 0f);
    //}
}
