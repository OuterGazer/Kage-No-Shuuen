using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ProjectileMakibisi : ProjectileBase
{
    [SerializeField] GameObject extraMakibisiPrefab;
    [SerializeField] int amountThrown = 10;
    [SerializeField] float maxForce, minForce = 1f;
    [SerializeField] float maxAngle, minAngle = 1f;

    private void OnEnable()
    {
        GameObject[] thrownMakibisis = new GameObject[amountThrown];

        for(int i = 0; i < thrownMakibisis.Length; i++)
        {
            GameObject current = thrownMakibisis[i];
            current = Instantiate(extraMakibisiPrefab, transform.position, transform.rotation);

            float throwingStrength = Random.Range(maxForce, minForce);
            float throwingAngle = Random.Range(maxAngle, minAngle);

            Rigidbody tempRB = current.GetComponent<Rigidbody>();
            current.transform.localRotation *= Quaternion.Euler(0f, throwingAngle, 0f);
            tempRB.AddForce(current.transform.forward * throwingStrength, ForceMode.Impulse);
        }
    }
}
