using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCrouchingState : MonoBehaviour
{
    [SerializeField] CharacterRunningState runningState;
    [SerializeField] CharacterIdleState idleState;

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
                Debug.Log("Leaving Crouching State, Entering Idle State");
                this.enabled = false;
            }
        }
    }

    private void OnCrouch(InputValue inputValue)
    {
        if (this.enabled && 
            IsCrouchButtonReleased(inputValue))
        {
            runningState.enabled = true;
            Debug.Log("Leaving Crouching State, Entering Running State");
            this.enabled = false;
        }
    }

    private static bool IsCrouchButtonReleased(InputValue inputValue)
    {
        return Mathf.Approximately(inputValue.Get<float>(), 0f);
    }
}
