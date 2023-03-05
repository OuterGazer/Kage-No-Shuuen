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
    [SerializeField] float correctionCoefficientZ = 0.25f;
    [SerializeField] float correctionCoefficientX = 0.23f;
    [SerializeField] CinemachineVirtualCamera stealthKillCamera;

    [HideInInspector] public UnityEvent onStealthKill;

    private float stealthKillRadius = 3f;

    private void OnEnable()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, stealthKillRadius, targetLayerMask);
        if(targets.Length > 0)
        {
            CharacterAnimator targetAnimator;
            DamageableWithLife damageable;
            GetTargetReferences(targets, out targetAnimator, out damageable);

            CorrectPlayerRelativePositionToTarget(targetAnimator);

            SwitchToStealthCamera();

            PlayAnimations(targetAnimator);

            damageable?.ReceiveStealthKill();
        }

    }

    private void OnDisable()
    {
        SwitchToFreeLookCamera();
    }

    private void PlayAnimations(CharacterAnimator targetAnimator)
    {
        onStealthKill.Invoke();
        targetAnimator?.PlayStealthDeathAnimation();
    }

    private void CorrectPlayerRelativePositionToTarget(CharacterAnimator targetAnimator)
    {
        transform.position = targetAnimator.transform.position + correctionCoefficientZ * targetAnimator.transform.forward;
        transform.LookAt(targetAnimator.transform.position);

        transform.position += correctionCoefficientX * transform.right;
    }

    private void SwitchToStealthCamera()
    {
        stealthKillCamera.gameObject.SetActive(true);
    }

    private void GetTargetReferences(Collider[] targets, out CharacterAnimator targetAnimator, out DamageableWithLife damageable)
    {
        targetAnimator = targets[0].GetComponentInParent<CharacterAnimator>();
        damageable = (DamageableWithLife)targets[0]?.GetComponent<IDamagereceiver>();
        targets[0].GetComponentInParent<SoldierBehaviour>().enabled = false;
    }

    private void SwitchToFreeLookCamera()
    {
        stealthKillCamera.gameObject.SetActive(false);
    }
}
