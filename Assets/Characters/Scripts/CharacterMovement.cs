using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterInput), typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float speed = 3f;

    private CharacterController characterController;
    private CharacterInput characterInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        characterInput = GetComponent<CharacterInput>();
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(characterInput.MovementDirection * speed * Time.deltaTime);
    }
}
