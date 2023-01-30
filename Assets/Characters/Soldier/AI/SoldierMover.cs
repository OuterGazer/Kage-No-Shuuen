using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierMover : MonoBehaviour
{
    private Hearing hearing;

    private MovementProperties movementProperties = new();
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        hearing = GetComponent<Hearing>();
        navMeshAgent= GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        hearing.onHeardNoiseEmitter.AddListener(TurnCharacterTowardsNoiseEmissionSource);
    }

    private void OnDisable()
    {
        hearing.onHeardNoiseEmitter.RemoveListener(TurnCharacterTowardsNoiseEmissionSource);
    }

    private void TurnCharacterTowardsNoiseEmissionSource(NoiseEmitter noiseEmitter)
    {
        Vector3 noiseSourceDirection = (noiseEmitter.transform.position - transform.position).normalized;
        float viewAngleFromNoiseSource = Mathf.Acos(Vector3.Dot(noiseSourceDirection, transform.forward)) * Mathf.Rad2Deg;

        if(viewAngleFromNoiseSource > 20f)
            transform.LookAt(noiseEmitter.transform.position);
    }
}
