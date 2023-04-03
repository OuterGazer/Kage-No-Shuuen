using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDeadState : CharacterStateBase
{
    [SerializeField] AudioClip deathSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (charController)
        {
            audioSource.PlayOneShot(deathSound);
            charController.detectCollisions = false;
            gameObject.layer = 0;
        }
    }


    private void Update()
    {
        if (charController.enabled)
        { UpdateMovement(speed, movementDirection, Vector3.up); }
    }

    private void OnDisable()
    {
        SetUnfocusedCameraOnDeath();
    }
}
