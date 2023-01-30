using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollisionEnter : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
