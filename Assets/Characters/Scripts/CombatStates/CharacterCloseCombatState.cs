using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterCloseCombatState : CharacterStateBase
{
    private bool slash = false;
    private bool heavySlash = false;

    [HideInInspector] public UnityEvent onSlash;
    [HideInInspector] public UnityEvent onHeavySlash;

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

    private void OnSlash() { slash = true; }
    private void OnHeavySlash() { heavySlash = true; }
}
