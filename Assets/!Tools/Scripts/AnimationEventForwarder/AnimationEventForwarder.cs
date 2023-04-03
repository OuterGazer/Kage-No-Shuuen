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

    public void ForwardSetInvincibility(int isInvincible)
    {
        SendMessageUpwards("SetInvincibility", isInvincible, SendMessageOptions.DontRequireReceiver);
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

    public void ForwardExitStealthKillState()
    {
        SendMessageUpwards("ExitStealthKillState");
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

    public void ForwardExitCloseCombatState()
    {
        SendMessageUpwards("ExitCloseCombatState");
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

    public void ForwardReadyToShoot()
    {
        SendMessageUpwards("ReadyToShoot", SendMessageOptions.DontRequireReceiver);
    }

    public void ForwardReleaseBowstring()
    {
        SendMessageUpwards("ReleaseBowstring");
    }

    public void ForwardShootBow()
    {
        SendMessageUpwards("ShootBow");
    }

    public void ForwardReloadBow()
    {
        SendMessageUpwards("ReloadBow");
    }

    public void ForwardThrowWeapon()
    {
        SendMessageUpwards("ThrowWeapon");
    }

    public void ForwardExitThrowingState()
    {
        SendMessageUpwards("ExitThrowingState", SendMessageOptions.DontRequireReceiver);
    }

    public void ForwardExitHitState()
    {
        SendMessageUpwards("ExitHitState");
    }

    [SerializeField] AudioClip runGravel;
    [SerializeField] AudioClip runWood;
    [SerializeField] AudioClip walkGravel;
    [SerializeField] AudioClip walkWood;
    [SerializeField] AudioClip fallOnGroundSound;
    [SerializeField] AudioClip swordExecutionSound;
    [SerializeField] AudioClip weaponlessExecutionSound;
    [SerializeField] AudioClip dieSound;



    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void ChooseRandomPitch()
    {
        audioSource.pitch = Random.Range(0.8f, 1.0f);
    }

    private void PlayFallOnGroundSound()
    {
        audioSource.PlayOneShot(fallOnGroundSound);
    }

    private void PlayRunningSound()
    {
        if (!audioSource.isPlaying)
        {
            ChooseRandomPitch();
            audioSource.PlayOneShot(runWood);
        }
    }

    private void PlayWalkingSound()
    {
        if (!audioSource.isPlaying)
        {
            ChooseRandomPitch();
            audioSource.PlayOneShot(walkWood);
        }
    }

    private void PlaySwordExecutionSound()
    {
        if (!audioSource.isPlaying)
        {
            ChooseRandomPitch();
            audioSource.PlayOneShot(swordExecutionSound);
        }
    }

    private void PlayWeaponlessExecutionSound()
    {
        if (!audioSource.isPlaying)
        {
            ChooseRandomPitch();
            audioSource.PlayOneShot(weaponlessExecutionSound);
        }
    }

    private void PlayDieSound()
    {
        if (!audioSource.isPlaying)
        {
            ChooseRandomPitch();
            audioSource.PlayOneShot(dieSound);
        }
    }
}
