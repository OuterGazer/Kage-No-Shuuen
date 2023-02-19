using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class DecisionMaker : MonoBehaviour
{
    Sight sight;
    Hearing hearing;

    Transform noiseEmitterTransform;
    Transform currentTarget;

    [SerializeField] float decisionReconsiderationFrequency = 1f;

    [HideInInspector] public UnityEvent<Transform> OnPlayerSeen;
    [HideInInspector] public UnityEvent OnTargetLost;

    private void Awake()
    {
        hearing = GetComponent<Hearing>();
        hearing.onHeardNoiseEmitter.AddListener(OnHeardNoiseEmitter);
        hearing.onForgotNoiseEmitter.AddListener(OnForgotNoiseEmitter);

        sight = GetComponent<Sight>();
        // TODO: añadir evento de actualización de target como en hearing
        // TODO: reaccionar a los eventos
    }

    private void OnDestroy()
    {
        hearing.onHeardNoiseEmitter.RemoveListener(OnHeardNoiseEmitter);
        hearing.onForgotNoiseEmitter.RemoveListener(OnForgotNoiseEmitter);
    }

    Coroutine reconsiderDecisionsCoroutine;
    private void OnEnable()
    {
        reconsiderDecisionsCoroutine = StartCoroutine(ReconsiderDecisions());
    }

    private void OnDisable()
    {
        StopCoroutine(reconsiderDecisionsCoroutine);
    }

    IEnumerator ReconsiderDecisions()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionReconsiderationFrequency);
            MakeDecision();
        }
    }

    void OnHeardNoiseEmitter(NoiseEmitter noiseEmitter)
    {
        noiseEmitterTransform = noiseEmitter.transform;
        MakeDecision();
    }

    void OnForgotNoiseEmitter()
    {
        noiseEmitterTransform = null;
    }

    void MakeDecision()
    {
        currentTarget = DecideNewTarget();
        if(currentTarget != null)
        {
            switch(currentTarget.tag)
            {
                case "Player":
                    OnPlayerSeen.Invoke(currentTarget);
                    break;
                case "Coin":
                    //aiGoToTargetState.SetTarget(currentTarget);
                    //SetCurrentState(aiGoToTargetState);
                    // TODO: implement event for interesting object for the BT
                    break;
                case "Axis":
                case "Allies":
                    //if (isOppositeSide(currentTarget.tag))
                    //{
                    //    if (SiEstaPaPegarleUnTiro())
                    //    {
                    //        Dispara();
                    //    }
                    //    else
                    //    {
                    //        Acercate();
                    //    }
                    //}
                    //SetCurrentState(aiPatrollingState);
                    break;
            }
        }
        else
        {
            //SetCurrentState(aiPatrollingState);
            OnTargetLost.Invoke();
        }
    }

    private Transform DecideNewTarget()
    {
        Transform target = null; 
        if(sight.interestingTargets.Length > 0)
        {
            target = sight.interestingTargets[0]?.transform;
        }
        else if (noiseEmitterTransform)
        {
            target = noiseEmitterTransform;
        }

        return target;
    }
}
