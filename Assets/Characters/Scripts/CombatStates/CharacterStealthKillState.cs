using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStealthKillState : CharacterStateBase
{
    [Header("Stealth Kill Settings")]
    [SerializeField] LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] float correctionCoefficientZOneHand = 0.25f;
    [SerializeField] float correctionCoefficientXOneHand = 0.23f;
    [SerializeField] float correctionCoefficientZTwoHand = 0.25f;
    [SerializeField] float correctionCoefficientXTwoHand = 0.23f;
    [SerializeField] float correctionCoefficientZProjectile = 0.25f;
    [SerializeField] float correctionCoefficientXProjectile = 0.23f;
    [SerializeField] CinemachineVirtualCamera stealthKillCamera;

    [HideInInspector] public UnityEvent onStealthKill;

    private static Weapon currentWeapon;
    private CharacterAnimator targetAnimator;
    private AnimationClip originalClip;

    private float stealthKillRadius = 3f;
    private float correctionCoefficientZ = 0.25f;
    private float correctionCoefficientX = 0.23f;

    public static void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void OnEnable()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, stealthKillRadius, targetLayerMask);
        if(targets.Length > 0)
        {
            DamageableWithLife damageable;
            GetTargetReferences(targets, out damageable);

            CorrectPlayerRelativePositionToTarget(targetAnimator);

            SwitchToStealthCamera();

            PlayAnimations(targetAnimator);

            damageable?.ReceiveStealthKill();
        }
    }

    private void GetTargetReferences(Collider[] targets, out DamageableWithLife damageable)
    {
        targetAnimator = targets[0].GetComponentInParent<CharacterAnimator>();
        damageable = (DamageableWithLife)targets[0]?.GetComponent<IDamagereceiver>();
        targets[0].GetComponentInParent<SoldierBehaviour>().enabled = false;
        targets[0].enabled = false;
    }



    private void CorrectPlayerRelativePositionToTarget(CharacterAnimator targetAnimator)
    {
        SetCorrectionCoefficientsDependingOnWeapon();

        transform.position = targetAnimator.transform.position + correctionCoefficientZ * targetAnimator.transform.forward;
        transform.LookAt(targetAnimator.transform.position);

        transform.position += correctionCoefficientX * transform.right;
    }

    private void SetCorrectionCoefficientsDependingOnWeapon()
    {
        // Not escalable, change if we intend to have more animations
        if (currentWeapon.IsProjectile)
        {
            correctionCoefficientZ = correctionCoefficientZProjectile;
            correctionCoefficientX = correctionCoefficientXProjectile;
        }
        else if (currentWeapon.Is2H)
        {
            correctionCoefficientZ = correctionCoefficientZTwoHand;
            correctionCoefficientX = correctionCoefficientXTwoHand;
        }
        else
        {
            correctionCoefficientZ = correctionCoefficientZOneHand;
            correctionCoefficientX = correctionCoefficientXOneHand;
        }
    }

    private void SwitchToStealthCamera()
    {
        stealthKillCamera.gameObject.SetActive(true);
    }

    private void PlayAnimations(CharacterAnimator targetAnimator)
    {
        AnimationClip victimAnimationClip = currentWeapon.AnimatorOverride["1-Back kill with sword-Actor1"];
        originalClip = targetAnimator.OverrideController["1-Back kill with sword-Actor1"];

        onStealthKill.Invoke();
        targetAnimator?.PlayStealthDeathAnimation(victimAnimationClip);
    }

    private void OnDisable()
    {
        if (originalClip)
        {
            targetAnimator.OverrideController["1-Back kill with sword-Actor1"] = originalClip;
        }

        SwitchToFreeLookCamera();
    }

    private void SwitchToFreeLookCamera()
    {
        stealthKillCamera.gameObject.SetActive(false);
    }
}
