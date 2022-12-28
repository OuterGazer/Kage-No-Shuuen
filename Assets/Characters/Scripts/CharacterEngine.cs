using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEngine : MonoBehaviour
{
    [SerializeField] private CharacterMovementBase[] movementStates = { };
    [SerializeField] private CharacterBlockingState blockingState;
    [SerializeField] private CharacterDodgingState dodgingState;

    [SerializeField] private CharacterMovementBase[] allowedStatesForBlocking = { };
    [SerializeField] private CharacterMovementBase[] allowedStatesForDodging = { };

    [SerializeField] private CharacterMovementBase currentMovementState; // Serialized for testing purposes


    private void Awake()
    {
        for (int i = 0; i < movementStates.Length; i++)
        {
            movementStates[i].onMovementStateChange.AddListener(UpdateCurrentMovementState);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < movementStates.Length; i++)
        {
            movementStates[i].onMovementStateChange.RemoveListener(UpdateCurrentMovementState);
        }
    }

    private void UpdateCurrentMovementState(CharacterMovementBase enablingState)
    {
        currentMovementState = enablingState;
    }

    public void OnBlock(InputValue inputValue)
    {
        float temp = inputValue.Get<float>();

        if (temp > 0f)
        {
            foreach(CharacterMovementBase state in allowedStatesForBlocking)
            {
                if (state.Equals(currentMovementState))
                    blockingState.enabled = true;
            }
        }
    }
}
