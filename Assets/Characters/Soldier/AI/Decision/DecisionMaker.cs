using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaker : MonoBehaviour
{
    Sight sight;
    Hearing hearing;

    Transform currentTarget;

    [SerializeField] float decisionReconsiderationFrequency = 1f;

    AIBaseState[] allStates;
    AIGoToTargetState aiGoToTargetState;
    AIPatrollingState aiPatrollingState;

    private void Awake()
    {
        hearing = GetComponent<Hearing>();
        hearing.onHeardNoiseEmitter.AddListener(OnHeardNoiseEmitter);

        sight = GetComponent<Sight>();
        // TODO: añadir evento de actualización de target como en hearing
        // TODO: reaccionar a los eventos

        allStates = GetComponents<AIBaseState>();
        aiGoToTargetState = GetComponent<AIGoToTargetState>();
        aiPatrollingState = GetComponent<AIPatrollingState>();
    }

    private void OnDestroy()
    {
        hearing.onHeardNoiseEmitter.RemoveListener(OnHeardNoiseEmitter);
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
        MakeDecision();
    }

    void MakeDecision()
    {
        currentTarget = DecideNewTarget();
        if(currentTarget != null)
        {
            switch(currentTarget.tag)
            {
                case "Coin":
                    aiGoToTargetState.SetTarget(currentTarget);
                    SetCurrentState(aiGoToTargetState);
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
                    SetCurrentState(aiPatrollingState);
                    break;
            }
        }
        else
        {
            SetCurrentState(aiPatrollingState);
        }
    }

    private Transform DecideNewTarget()
    {
        return sight.interestingTargets.Length <= 0 ? null : sight.interestingTargets[0].transform;
    }

    private void SetCurrentState(AIBaseState newCurrentState)
    {
        foreach(AIBaseState item in allStates)
        {
            if (IsNewStateDifferentFromCurrentState(newCurrentState, item))
                { item.enabled = true; }
            else if (IsNewStateEqualsToCurrentState(newCurrentState, item))
                { item.enabled = false; }
        }
    }

    private static bool IsNewStateDifferentFromCurrentState(AIBaseState newCurrentState, AIBaseState item)
    {
        return (item == newCurrentState) && (item.enabled == false);
    }

    private static bool IsNewStateEqualsToCurrentState(AIBaseState newCurrentState, AIBaseState item)
    {
        return (item != newCurrentState) && (item.enabled == true);
    }
}
