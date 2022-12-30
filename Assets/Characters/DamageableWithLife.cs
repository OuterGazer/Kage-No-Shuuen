using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableWithLife : MonoBehaviour, IDamagereceiver
{
    [SerializeField] float life;
    public float Life => life;
    [SerializeField] float coolDownTime = 0.5f;
    float lastTimeDamageWasReceived= 0f;

    public void ReceiveDamage(float damage)
    {
        if((Time.time - lastTimeDamageWasReceived) > coolDownTime)
        {
            lastTimeDamageWasReceived = Time.time;
            life -= damage;

            if(Life <= 0f) 
                { Destroy(gameObject); }
        }
    }
}
