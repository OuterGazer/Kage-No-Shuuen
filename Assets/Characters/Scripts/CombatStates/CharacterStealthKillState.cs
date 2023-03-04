using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStealthKillState : CharacterStateBase
{
    [SerializeField] LayerMask targetLayerMask = Physics.DefaultRaycastLayers;

    [HideInInspector] public UnityEvent onStealthKill;

    private float stealthKillRadius = 3f;

    private void OnEnable()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, stealthKillRadius, targetLayerMask);

        CharacterAnimator targetAnimator = targets[0].GetComponentInParent<CharacterAnimator>();

        onStealthKill.Invoke();
        targetAnimator?.PlayStealthDeathAnimation();
    }
}
