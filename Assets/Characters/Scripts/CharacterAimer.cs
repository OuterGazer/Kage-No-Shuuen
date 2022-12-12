using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(CharacterStateHandler))]
public class CharacterAimer : MonoBehaviour
{
    [SerializeField] float timeToOrientateCharacterForward = 0.05f;

    CharacterMovement characterMovement;
    Camera mainCamera;
    CharacterStateHandler characterStateHandler;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        mainCamera = Camera.main;
        characterStateHandler = GetComponent<CharacterStateHandler>();
    }

    private void Update()
    {
        if (IsCharacterMovingButNotOnWall())
        {
            if (characterStateHandler.PlayerState == CharacterState.OnHook)
            {
                transform.up = characterMovement.HangingDirection;
            }
            else if (characterStateHandler.PlayerState == CharacterState.OnAir)
            {
                transform.up = Vector3.up;
            }
            else
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

    private bool IsCharacterMovingButNotOnWall()
    {
        return Mathf.Abs(characterMovement.MovingSpeed) > 0.1f &&
                    !characterStateHandler.PlayerState.HasFlag(CharacterState.OnWall);
    }

    // TODO: find solution to throw hook animation not pointing to hook target
    //public void OnHookThrow()
    //{
    //    Vector3 target = characterMovement.HookTarget.position;
    //    Vector3 projectedTargetPosition = Vector3.ProjectOnPlane(target, Vector3.up);

    //    transform.forward = projectedTargetPosition;
    //}
}
