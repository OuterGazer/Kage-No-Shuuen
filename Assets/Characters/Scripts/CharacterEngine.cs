using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEngine : MonoBehaviour
{
    [SerializeField] private CharacterMovementBase[] possibleStates = { };

    private void Awake()
    {
        possibleStates = GetComponents<CharacterMovementBase>();
    }
}
