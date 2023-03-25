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
    [SerializeField] float maxLife;
    public float Life => maxLife;
    private float life = 0f;
    [SerializeField] float coolDownTime = 0.5f;
    float lastTimeDamageWasReceived= 0f;

    private bool isAlive = true;

    [HideInInspector] public UnityEvent OnGettingHit;
    [HideInInspector] public UnityEvent OnDying;

    void OnEnable()
    {
        life = maxLife;
        isAlive = true;
    }

    public void ReceiveDamage(float damage)
    {
        if (isAlive)
        {
            if ((Time.time - lastTimeDamageWasReceived) > coolDownTime)
            {
                lastTimeDamageWasReceived = Time.time;
                maxLife -= damage;

                if (maxLife <= 0f)
                {
                    OnDying?.Invoke();                    
                    isAlive = false;
                    ManageDeath();
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
            maxLife = 0;
            GetComponent<TrackedObject>()?.SetIsIndicatorVisible(false);
            ManageDeath();
            isAlive = false;
        }
    }

    private void ManageDeath()
    {
        PooledObject belongsToPool = GetComponentInParent<PooledObject>();
        if (belongsToPool)
        {
            belongsToPool.gameObject.ReturnToPool();
        }
        else
        {
            Destroy(parentToDestroy, timeToDestroyObject);
        }
    }
}
