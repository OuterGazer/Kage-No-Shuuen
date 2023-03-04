using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public enum Type 
    {
        Continuous,
        OnlyWhenRunning,
        OnDemand
    }

    [SerializeField] Type type = Type.Continuous;
    [SerializeField] float emissionFrequency = 5f;
    [SerializeField] float radius = 10f;
    [SerializeField] LayerMask listenersLayer = Physics.DefaultRaycastLayers;

    Vector3 lastPositionOnEmmit = Vector3.zero;

    Coroutine noiseEmissionCoroutine;

    private bool isCrouching = false;

    public void SetIsCrouching (bool isCrouching)
    {
        if (isCrouching)
        {
            this.isCrouching = isCrouching;
        }
        else
        {
            StartCoroutine(DelaySetIsCrouchingToFalse(isCrouching));
        }
    }

    private IEnumerator DelaySetIsCrouchingToFalse(bool isCrouching)
    {
        yield return new WaitUntil(() => transform.position == lastPositionOnEmmit); // Wait until movement smoothing comes to a halt
        this.isCrouching = isCrouching;
    }

    private void OnEnable()
    {
        if(type != Type.OnDemand)
        {
            noiseEmissionCoroutine = StartCoroutine(NoiseEmission());
            lastPositionOnEmmit = transform.position;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(noiseEmissionCoroutine);
    }

    IEnumerator NoiseEmission()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f / emissionFrequency)); // To avoid performance spikes of multiple emitters emitting always at the same exact time

        while (true)
        {
            yield return new WaitForSeconds(1f / emissionFrequency);

            bool performEmission = false;
            switch (type)
            {
                case Type.Continuous: performEmission = true; break;
                case Type.OnlyWhenRunning: performEmission = ((transform.position != lastPositionOnEmmit) && !isCrouching); break;
            }

            if (isCrouching)
            {
                lastPositionOnEmmit = transform.position;
            }

            if (performEmission)
                { EmitNoise(); Debug.Log("making noise!!"); }
        }
    }

    private void EmitNoise()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, listenersLayer);
        foreach(Collider item in colliders)
        {
            INoiseReceiver receiver = item.GetComponentInParent<INoiseReceiver>();
            receiver?.NotifyNoise(this);
        }

        lastPositionOnEmmit = transform.position;
    }
}
