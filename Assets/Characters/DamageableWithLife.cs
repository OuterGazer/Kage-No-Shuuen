using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableWithLife : MonoBehaviour, IDamagereceiver
{
    // TODO: implement this better, because for soldiers is in collisoin child and player is in top parent
    [SerializeField] GameObject objectToDestroy;

    // TODO: implement multiple hit avoiding through event system (specially for multiple raycast weapons)
    // TODO: Impement invincibilty by disabling character controller collider so throwing weapons don't disappear.
    [SerializeField] float life;
    public float Life => life;
    [SerializeField] float coolDownTime = 0.5f;
    float lastTimeDamageWasReceived= 0f;

    private bool isInvincible = false;

    public void ReceiveDamage(float damage)
    {
        if((Time.time - lastTimeDamageWasReceived) > coolDownTime)
        {
            if (!isInvincible)
            {
                lastTimeDamageWasReceived = Time.time;
                life -= damage;

                if (life <= 0f)
                { Destroy(objectToDestroy); }
            }
        }
    }

    // Called from dodging animation event
    public void SetInvincibility(int isInvincible)
    {
        this.isInvincible = (isInvincible == 1) ? true : false;
    }
}
