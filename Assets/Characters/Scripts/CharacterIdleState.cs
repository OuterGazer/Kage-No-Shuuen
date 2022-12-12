using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterIdleState : MonoBehaviour
{
    [SerializeField] CharacterRunningState runningState;

    private void Awake()
    {
        this.enabled = true;
        Debug.Log("Character is in Idle State");
    }


    public void OnMove(InputValue inputValue)
    {
        if (this.enabled)
        {
            if (inputValue.Get<Vector2>() != Vector2.zero)
            {
                runningState.enabled = true;
                Debug.Log("Leaving Idle State, Entering Running State");
                this.enabled = false;
            }
        }
    }
}
