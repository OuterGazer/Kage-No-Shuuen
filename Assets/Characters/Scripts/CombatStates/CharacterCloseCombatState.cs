using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterCloseCombatState : CharacterStateBase
{
    [SerializeField] float stepForwardLength = 1f;

    private bool slash = false;
    private bool heavySlash = false;
    private bool shouldStepForward = false;

    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;

    private void Update()
    {
        UpdateSlash();

        if (shouldStepForward) 
        { PushCharacterForward(stepForwardLength); }
    }

    private void UpdateSlash()
    {
        if (slash)
        {
            onSlash.Invoke();
            slash = false;
        }
        else if (heavySlash)
        {
            onHeavySlash.Invoke();
            heavySlash = false;
        }
    }

    // Methods called from animation events
    private void OnSlash() { slash = true; }
    private void OnHeavySlash() { heavySlash = true; }
    private void DamageStart() { shouldStepForward = true; }
    private void DamageEnd() { shouldStepForward = false; }
}
