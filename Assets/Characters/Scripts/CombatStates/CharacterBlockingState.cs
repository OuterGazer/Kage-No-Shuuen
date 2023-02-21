using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class CharacterBlockingState : CharacterStateBase
{
    [Header("Rigging Settings")]
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] Rig blockingRig;
    [SerializeField] ChainIKConstraint torsoConstraint;
    [SerializeField] TwoBoneIKConstraint rightArmConstraint;
    [SerializeField] TwoBoneIKConstraint lefttArmConstraint;
    [SerializeField] Transform torsoTarget_1H;
    [SerializeField] Transform torsoTarget_2H;
    [SerializeField] Transform rightArmTarget_1H;
    [SerializeField] Transform leftArmTarget_1H;
    [SerializeField] Transform rightArmTarget_2H;
    [SerializeField] Transform leftArmTarget_2H;

    private bool is2HWeapon = false;
    private bool isRigSet = false;
    private bool isBlocking = false;

    [HideInInspector] public UnityEvent<bool> UpdateBlockingStatus; // Event for the animator

    public void SetIs2HWeapon(bool is2HWeapon)
    {
        this.is2HWeapon = is2HWeapon;
    }

    private void OnEnable()
    {
        UpdateBlockingStatus.Invoke(true);
        isBlocking = true;
    }

    private void OnDisable()
    {
        EraseRigTargets();

        UpdateBlockingStatus.Invoke(false);
        isBlocking = false;
        isRigSet = false;
        blockingRig.weight = 0;
    }

    private void EraseRigTargets()
    {
        AssignRigTargets(null, null, null);
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);

        SetProperRigTargets();

        ChangeBlockingRiggingWeight();
    }

    private void SetProperRigTargets()
    {
        if (!isRigSet)
        {
            if (!is2HWeapon)
            {
                AssignRigTargets(torsoTarget_1H, rightArmTarget_1H, leftArmTarget_1H);
            }
            else
            {
                AssignRigTargets(torsoTarget_2H, rightArmTarget_2H, leftArmTarget_2H);
            }

            rigBuilder.Build();
            isRigSet = true;
        }
    }

    private void AssignRigTargets(Transform torso, Transform rightArm, Transform leftArm)
    {
        torsoConstraint.data.target = torso;
        rightArmConstraint.data.target = rightArm;
        lefttArmConstraint.data.target = leftArm;
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

            isRigSet = false;
        }
        else
        {
            if (blockingRig.weight <= 1f)
                blockingRig.weight += weightChangeAcceleration * Time.deltaTime;
            else
                blockingRig.weight = 1f;
        }
    }

    //public override void ExitState()
    //{
    //    StartCoroutine(OnExitState());
    //}

    // Method necessary to make unblocking animation look better
    //private IEnumerator OnExitState() 
    //{
    //    UpdateBlockingStatus.Invoke(false);
    //    isBlocking = false;
    //    combatStateSpeedModifier = 0f;

    //    yield return new WaitUntil(() => blockingRig.weight <= 0f);
    //}
}
