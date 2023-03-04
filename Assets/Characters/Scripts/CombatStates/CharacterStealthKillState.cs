using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStealthKillState : CharacterStateBase
{
    [HideInInspector] public UnityEvent onStealthKill;

    private void OnEnable()
    {
        onStealthKill.Invoke();
    }
}
