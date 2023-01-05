using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationEventForwarder : MonoBehaviour
{
    // TODO: Arreglar todo esto con eventos más limpios

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

    public void ForwardSetIsSlashing(int isSlashing)
    {
        SendMessageUpwards("SetIsSlashing", isSlashing);
    }
}
