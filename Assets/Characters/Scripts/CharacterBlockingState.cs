using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CharacterBlockingState : CharacterMovementBase
{
    [SerializeField] Rig blockingRig;
    private bool isBlocking = false;

    [HideInInspector] public UnityEvent<bool> UpdateBlockingStatus;

    private void Awake()
    {
        this.enabled = false;
    }

    private void Update()
    {
        ChangeBlockingRiggingWeight();

        UpdateMovement(speed, movementDirection, Vector3.up);

        OrientateCharacterForward();
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

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (temp > 0f)
        {
            UpdateBlockingStatus.Invoke(true);
            isBlocking = true;
            this.enabled = true;
        }

        else
        {
            UpdateBlockingStatus.Invoke(false);
            isBlocking = false;
            StartCoroutine(DisableScript());
        }
    }

    private IEnumerator DisableScript()
    {
        yield return new WaitUntil(() => blockingRig.weight <= 0f);

        this.enabled = false;        
    }
}
