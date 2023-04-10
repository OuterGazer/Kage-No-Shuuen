using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMakibisi : ProjectileBase
{
    [SerializeField] GameObject extraMakibisiPrefab;
    [SerializeField] int amountThrown = 10;
    [SerializeField] float maxForce, minForce = 1f;
    [SerializeField] float maxAngle, minAngle = 1f;
    [SerializeField] float lifeSpan = 5f;

    Collider[] colliders = new Collider[50];

    // TODO: implement an Object Pool for this and clean the collider disabling/enabling
    private void OnEnable()
    {
        GameObject[] thrownMakibisis = new GameObject[amountThrown];

        for(int i = 0; i < thrownMakibisis.Length; i++)
        {
            GameObject current = Instantiate(extraMakibisiPrefab, transform.position, transform.rotation);
            thrownMakibisis[i] = current;
            
            colliders[i] = thrownMakibisis[i].GetComponentInChildren<Collider>(); // Need to do this so makibisis don't collide among themselves and create buggy behaviour
            colliders[i].enabled = false;

            float throwingStrength = Random.Range(maxForce, minForce);
            float throwingAngle = Random.Range(maxAngle, minAngle);

            Rigidbody tempRB = current.GetComponent<Rigidbody>();
            current.transform.localRotation *= Quaternion.Euler(0f, throwingAngle, 0f);
            tempRB.AddForce(current.transform.forward * throwingStrength, ForceMode.Impulse);
        }

        StartCoroutine(DisableThrownMakibisis(thrownMakibisis));
    }

    private IEnumerator DisableThrownMakibisis(GameObject[] thrownMakibisis)
    {
        yield return new WaitForSeconds(0.4f);

        foreach(Collider item in colliders)
        {
            item.enabled = true;
        }

        yield return new WaitForSeconds(lifeSpan);

        foreach(GameObject item in thrownMakibisis)
        {
            Destroy(item);
        }

        Destroy(this.gameObject);
    }
}
