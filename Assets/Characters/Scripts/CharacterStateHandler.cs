using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CharacterStateHandler : MonoBehaviour
{
    private CharacterState playerState;
    public CharacterState PlayerState => playerState;

    private void Start()
    {
        playerState = CharacterState.Idle;
    }

    public void OnCrouch(InputValue inputValue)
    {
        if (IsCrouchButtonPressed(inputValue))
        {
            if (playerState.HasFlag(CharacterState.OnWall))
            {
                BroadcastMessage("HaveCharacterInteractWithWall", false);
                playerState = CharacterState.Idle | CharacterState.Crouching;
            }
            if (playerState == CharacterState.Idle)
            { playerState = CharacterState.Idle | CharacterState.Crouching; }
            else if (playerState == CharacterState.Running)
            { playerState = CharacterState.Crouching; }
        }
        else
        {
            if (playerState.HasFlag(CharacterState.OnWall))
            { } // Do nothing for now
            else if (playerState.HasFlag(CharacterState.Idle))
            { playerState = CharacterState.Idle; }
            else if (playerState == CharacterState.Crouching)
            { playerState = CharacterState.Running; }
        }
    }

    private static bool IsCrouchButtonPressed(InputValue inputValue)
    {
        return !Mathf.Approximately(inputValue.Get<float>(), 0f);
    }

    public void OnMove(InputValue inputValue)
    {
        if (inputValue.Get<Vector2>() != Vector2.zero)
        {
            if (playerState.HasFlag(CharacterState.Idle))
            {
                if (playerState.HasFlag(CharacterState.OnWall))
                    { playerState = CharacterState.Crouching | CharacterState.OnWall; }
                else if (playerState.HasFlag(CharacterState.Crouching))
                    { playerState = CharacterState.Crouching; }
                else
                    { playerState = CharacterState.Running; }
            }
        }
        else
        {
            if (playerState == CharacterState.Running)
                { playerState = CharacterState.Idle; }
            else if ((playerState.HasFlag(CharacterState.OnWall)))
                { playerState = CharacterState.Idle | CharacterState.Crouching | CharacterState.OnWall; }
            else if (playerState == CharacterState.Crouching)
                { playerState = CharacterState.Idle | CharacterState.Crouching; }
        }
    }

    [SerializeField] float timeToChangeFromWallToFreeMove = 0.25f;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Cover"))
        {
            if (playerState.HasFlag(CharacterState.Crouching))
            {
                playerState = CharacterState.Idle | CharacterState.Crouching | CharacterState.OnWall;
                BroadcastMessage("HaveCharacterInteractWithWall", true);

                DOTween.To(() => transform.forward, x => transform.forward = x, hit.normal, timeToChangeFromWallToFreeMove);
            }
        }
    }
}
