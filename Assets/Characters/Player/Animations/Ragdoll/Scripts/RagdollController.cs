using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class RagdollController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 5.0f;
    [SerializeField] float upwardsModifier = 5.0f;

    private Animator animator;
    private PlayerInput playerInput;
    private CharacterController charController;
    private CharacterAnimator charAnimator;
    private StateController stateController;

    [Header("Miscellanous")]
    [SerializeField] CinemachineVirtualCamera explosionCamera;
    [SerializeField] Transform cameraTarget;

    private Rigidbody[] limbsRB;
    private Collider[] limbsCol;

    public bool debugRagdoll = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();
        charController = GetComponentInParent<CharacterController>();
        charAnimator = GetComponentInParent<CharacterAnimator>();
        stateController = GetComponentInParent<StateController>();
        limbsRB = GetComponentsInChildren<Rigidbody>();
        limbsCol = GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        foreach(Rigidbody item in limbsRB) { item.isKinematic = true; }
        foreach(Collider item in limbsCol) { item.enabled = false; }
    }

    private void Update()
    {
        if (debugRagdoll)
        {
            ActivateRagdoll();
            ApplyExplosionForceToRagdoll(transform.position + 2f * Vector3.forward);
            debugRagdoll = false;
        }
    }

    public void ActivateRagdoll()
    {
        animator.enabled = false;
        playerInput.enabled = false;
        charController.enabled = false;
        charAnimator.enabled = false;
        stateController.enabled = false; 

        foreach (Rigidbody item in limbsRB) { item.isKinematic = false; }
        foreach (Collider item in limbsCol) { item.enabled = true; }
    }

    public void ApplyExplosionForceToRagdoll(Vector3 explosionPosition)
    {
        if (explosionCamera) { explosionCamera.gameObject.SetActive(true); }

        foreach(Rigidbody item in limbsRB)
        {
            item.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
    }
}
