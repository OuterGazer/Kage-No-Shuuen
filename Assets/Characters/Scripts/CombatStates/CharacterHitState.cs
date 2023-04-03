using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHitState : CharacterStateBase
{
    [SerializeField] AudioClip hitSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (charController)
        { charController.detectCollisions = false; audioSource.PlayOneShot(hitSound); }
    }

    private void OnDisable() 
    { 
        if (charController)
        { charController.detectCollisions = true; } 
    }

    private void Update()
    {
        UpdateMovement(speed, movementDirection, Vector3.up);
    }
}
