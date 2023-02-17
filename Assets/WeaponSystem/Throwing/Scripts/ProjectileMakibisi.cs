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
    [SerializeField] float lifeSpan = 5f;


    // TODO: implement an Object Pool for this
    private void OnEnable()
    {
        GameObject[] thrownMakibisis = new GameObject[amountThrown];

        for(int i = 0; i < thrownMakibisis.Length; i++)
        {
            GameObject current = Instantiate(extraMakibisiPrefab, transform.position, transform.rotation);            
            thrownMakibisis[i] = current;
            thrownMakibisis[i].GetComponent<ProjectileBase>().SetOwnerTag(tag);

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
        yield return new WaitForSeconds(lifeSpan);

        foreach(GameObject item in thrownMakibisis)
        {
            Destroy(item);
        }

        Destroy(this.gameObject);
    }
}
