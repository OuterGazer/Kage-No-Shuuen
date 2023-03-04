using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageableWithLife : MonoBehaviour, IDamagereceiver
{
    // TODO: implement this better, because for soldiers is in collisoin child and player is in top parent
    //       this is due to enemies having the collider in a child gameobject and the player having teh collider in the character controller in the top parent
    [SerializeField] GameObject parentToDestroy;
    [SerializeField] float timeToDestroyObject = 5f;

    // TODO: implement multiple hit avoiding through event system (specially for multiple raycast weapons)
    [SerializeField] float life;
    public float Life => life;
    [SerializeField] float coolDownTime = 0.5f;
    float lastTimeDamageWasReceived= 0f;

    private bool isAlive = true;

    [HideInInspector] public UnityEvent OnGettingHit;
    [HideInInspector] public UnityEvent OnDying;

    public void ReceiveDamage(float damage)
    {
        if (isAlive)
        {
            if ((Time.time - lastTimeDamageWasReceived) > coolDownTime)
            {
                lastTimeDamageWasReceived = Time.time;
                life -= damage;

                if (life <= 0f)
                {
                    OnDying?.Invoke();
                    Destroy(parentToDestroy, timeToDestroyObject);
                    isAlive = false;
                    return;
                }

                OnGettingHit?.Invoke();
            }
        }
    }

    public void ReceiveStealthKill()
    {
        if (isAlive)
        {
            life = 0;
            Destroy(parentToDestroy, timeToDestroyObject);
            isAlive = false;
        }
    }
}
