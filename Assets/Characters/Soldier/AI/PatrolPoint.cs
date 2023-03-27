using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
