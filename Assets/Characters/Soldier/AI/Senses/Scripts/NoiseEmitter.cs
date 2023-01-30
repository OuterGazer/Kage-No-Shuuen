using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public enum Type 
    {
        Continuous,
        OnlyWhenMoving,
        OnDemand
    }

    [SerializeField] Type type = Type.Continuous;
    [SerializeField] float emissionFrequency = 5f;
    [SerializeField] float radius = 10f;
    [SerializeField] LayerMask listenersLayer = Physics.DefaultRaycastLayers;

    Vector3 lastPositionOnEmmit = Vector3.zero;

    Coroutine noiseEmissionCoroutine;

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
                case Type.OnlyWhenMoving: performEmission = (transform.position != lastPositionOnEmmit); break;
            }

            if (performEmission)
                { EmitNoise(); }
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
