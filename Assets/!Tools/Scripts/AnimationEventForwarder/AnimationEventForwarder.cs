using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    
    public void ForwardHookHasArrivedAtTarget()
    {
        SendMessageUpwards("HookHasArrivedAtTarget");
    }

    public void ForwardExitDodgingState()
    {
        SendMessageUpwards("ExitDodgingState");
    }

    public void ForwardChangeCurrentWeapon()
    {
        SendMessageUpwards("ChangeCurrentWeapon");
    }

    public void ForwardResetWeaponChangeState()
    {
        SendMessageUpwards("ResetWeaponChangeState");
    }
    
    public void ForwardSetCanChainCombo(int canChain)
    {
        SendMessageUpwards("SetCanChainCombo", canChain);
    }
}
