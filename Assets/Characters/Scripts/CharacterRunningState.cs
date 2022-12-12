using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CharacterRunningState : MonoBehaviour
{
    [SerializeField] CharacterIdleState idleState;
    [SerializeField] CharacterCrouchingState crouchingState;

    private void Awake()
    {
        this.enabled = false;
    }

    // TODO: refactor this OnMove repeated code from CharacterRunningState
    void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            Vector3 inputBuffer = inputValue.Get<Vector2>();

            // Movement from Input Module sends only Vector3.up and Vector3.down movement and it needs to be corrected into forward and backward.
            if (inputBuffer.y != 0)
            {
                inputBuffer = new Vector3(inputBuffer.x, 0f, inputBuffer.y);
            }
            else if(inputBuffer == Vector3.zero)
            {
                idleState.enabled = true;
                Debug.Log("Leaving Running State, Entering Idle State");
                this.enabled = false;
            }
        }        
    }

    private void OnCrouch(InputValue inputValue)
    {
        if (this.enabled)
        {
            crouchingState.enabled = true;
            Debug.Log("Leaving Running State, Entering Crouching State");
            this.enabled = false;
        }
    }
}
