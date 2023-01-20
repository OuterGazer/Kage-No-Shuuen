using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterCloseCombatState : CharacterStateBase
{
    private bool slash = false;
    private bool heavySlash = false;
    private bool canChainCombo = false;

    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;

    private void Awake()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        onCombatStateEnteringOrExiting.Invoke(this);
    }

    private void Update()
    {
        UpdateSlash();
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

    // Called from an animation event
    private void SetCanChainCombo(int canChainCombo)
    {
        if (canChainCombo == 1)
        { this.canChainCombo = true; }
        else if (canChainCombo == 0)
        { this.canChainCombo = false; }
    }

    // Called from animation event
    public void ExitCloseCombatState() 
    {
        if (!canChainCombo)
        { onCombatStateEnteringOrExiting.Invoke(null); }
        
    }

    private void OnSlash() { slash = true; }
    private void OnHeavySlash() { heavySlash = true; }
}
