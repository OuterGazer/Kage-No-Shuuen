using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Music Tracks")]
    [SerializeField] AudioClip mainMenuTrack;
    [SerializeField] AudioClip stealthTrack1;
    [SerializeField] AudioClip combatTrack;
    [SerializeField] AudioClip enemyWavesTrack;
    [SerializeField] AudioClip victoryMusic;

    [Header("Play Settings")]
    [SerializeField] float delayedPlayTime = 3f;
    public bool isIncombat = false;
    private float combatCounter = 0f;
    [SerializeField] float returnToStealthMusicTimeUponCombatEnd = 3f;

    private AudioSource audioSource;
    private AudioClip currentClip;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerVolume = true;
    }

    private void Start()
    {
        currentClip = mainMenuTrack;
        audioSource.clip = currentClip;
        audioSource.PlayDelayed(delayedPlayTime);
    }

    private void Update()
    {
        if (isIncombat)
        {
            combatCounter += Time.deltaTime;

            if (combatCounter >= returnToStealthMusicTimeUponCombatEnd)
            { isIncombat = false; }
        }
    }

    public void PauseMusic()
    {
        audioSource.Pause();
        AudioListener.pause = true;
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
        AudioListener.pause = false;
    }

    public void FadeOutCurrentTrack(float fadeTime)
    {
        audioSource.DOFade(0f, fadeTime);
        AudioListener.volume = 0f;
    }

    public void ChangeToGameTrack()
    {
        currentClip = stealthTrack1;
        audioSource.clip = currentClip;

        audioSource.Play();

        audioSource.DOFade(1f, 3f);
        AudioListener.volume = 1f;
    }

    public void ChangeToCombatMusic(bool isRegularCombat)
    {
        if(currentClip == combatTrack) { combatCounter = 0f; return; }
            else if(currentClip == enemyWavesTrack) { return; }

        isIncombat = true;
        currentClip = isRegularCombat ? combatTrack : enemyWavesTrack;
        currentClip = combatTrack;
        audioSource.clip = currentClip;
        audioSource.Play();
    }

    public void ReturnToStealthMusic(float fadeTime)
    {
        if (currentClip == stealthTrack1) { return; }
        if (isIncombat) { return; }

        audioSource.DOFade(0f, fadeTime).OnComplete(() => ChangeToGameTrack());
    }

    public void PlayVictoryMusic()
    {
        audioSource.DOFade(0f, 1f);
        currentClip = victoryMusic;
        audioSource.clip = currentClip;
        audioSource.DOFade(1f, 1f);
        audioSource.Play();
    }
}
