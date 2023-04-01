using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.AI;

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
    private NavMeshAgent navMeshAgent;
    private SoldierBehaviour soldierBehaviour;

    [Header("Miscellanous")]
    [SerializeField] CinemachineVirtualCamera explosionCamera;
    [SerializeField] Transform cameraTarget;

    private Rigidbody[] limbsRB;
    private Collider[] limbsCol;

    [Header("Debug")]
    public bool debugRagdoll = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();
        charController = GetComponentInParent<CharacterController>();
        charAnimator = GetComponentInParent<CharacterAnimator>();
        stateController = GetComponentInParent<StateController>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        soldierBehaviour = GetComponentInParent<SoldierBehaviour>();
        limbsRB = GetComponentsInChildren<Rigidbody>();
        limbsCol = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        animator.enabled = true;
        charAnimator.enabled = true;
        foreach(Rigidbody item in limbsRB) { item.isKinematic = true; }
        foreach(Collider item in limbsCol) { item.enabled = false; }

        if (CompareTag("Player"))
        {
            explosionCamera.gameObject.SetActive(false);
            playerInput.enabled = true;
            charController.enabled = true;
            stateController.enabled = true;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (debugRagdoll)
        {
            ActivateRagdoll();
            ApplyExplosionForceToRagdoll(transform.position + 2f * Vector3.forward);
            debugRagdoll = false;
        }
#endif
    }

    public void ActivateRagdoll()
    {
        animator.enabled = false;
        charAnimator.enabled = false;
        if (CompareTag("Player"))
        {
            playerInput.enabled = false;
            charController.enabled = false;
            stateController.enabled = false;
        }
        else if (CompareTag("Soldier"))
        {
            navMeshAgent.enabled = false;
            soldierBehaviour.enabled = false;
        }        

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
