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

    public void ForwardDamageStart()
    {
        SendMessageUpwards("DamageStart");
    }

    public void ForwardDamageEnd()
    {
        SendMessageUpwards("DamageEnd");
    }

    public void ForwardSpawnArrowInHand()
    {
        SendMessageUpwards("SpawnArrowInHand");
    }

    public void ForwardPullBowstring()
    {
        SendMessageUpwards("PullBowstring");
        SendMessageUpwards("SpawnArrowInBow");
    }

    public void ForwardReleaseBowstring()
    {
        SendMessageUpwards("ReleaseBowstring");
    }

    public void ForwardShootBow()
    {
        SendMessageUpwards("ShootBow");
    }

    public void ForwardExitShooting()
    {
        SendMessageUpwards("ExitShooting");
    }

    public void ForwardThrowWeapon()
    {
        SendMessageUpwards("ThrowWeapon");
    }
}
