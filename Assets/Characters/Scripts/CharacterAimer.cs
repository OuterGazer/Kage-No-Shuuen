using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterAimer : MonoBehaviour
{
    [SerializeField] float timeToOrientateCharacterForward = 0.05f;

    CharacterMovement characterMovement;
    Camera mainCamera;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mathf.Abs(characterMovement.MovingSpeed) > 0.1f)
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);
        }
    }
}
