using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectDisabler : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToDisable;

    private Collider disablingTrigger;

    public UnityEvent onDisablingObjects;

    private void Awake()
    {
        disablingTrigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject item in objectsToDisable) 
            {
                item?.SetActive(false);
            }
            disablingTrigger.enabled = false;
        }
        onDisablingObjects.Invoke();
    }
}
