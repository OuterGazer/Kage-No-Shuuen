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
        if (Mathf.Abs(characterMovement.MovingSpeed) > 0.1f &&
            !characterMovement.PlayerState.HasFlag(CharacterState.OnWall))
        {
            Vector3 projectedForwardVector = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);

            DOTween.To(() => transform.forward, x => transform.forward = x, projectedForwardVector, timeToOrientateCharacterForward);

            // Alternativa sin DoTween pero con un fallo
            // forwardOrientationSpeed lo tenía a 3f, sin embargo al ir en diagonal hacia atrás y mover mucho al personaje le terminaba viendo la cara.
            //Vector3 forwardVectorToLookAtThisFrame = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
            //Vector3 angleToOrientateCharacterThisFrame = Vector3.RotateTowards(transform.forward, forwardVectorToLookAtThisFrame, forwardOrientationSpeed * Time.deltaTime, 0f);
            //transform.rotation = Quaternion.LookRotation(angleToOrientateCharacterThisFrame, Vector3.up);
        }
    }
}
