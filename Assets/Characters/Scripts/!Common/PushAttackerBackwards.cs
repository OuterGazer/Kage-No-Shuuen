using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushAttackerBackwards : MonoBehaviour
{
    // TODO: have pushback not be an instant teleportation
    // TODO: have a system that avoids taking damage while blocking. Just in case, sometimes player gets hit, sometimes not.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soldier"))
        {
            other.GetComponentInParent<NavMeshAgent>()?.Move(1f*transform.forward);
        }
    }
}
