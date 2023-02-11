using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseState : MonoBehaviour
{
    void Awake()
    {
        InternalAwake();
    }

    // Start is called before the first frame update
    void Start()
    {
        InternalStart(); // Unity ejecuta siemrpe la versi�n de la derivada m�s externa, esta estructura permite que ejecutemos siempre justo el que queremos o en le orden que queramos
    }

    // Update is called once per frame
    void Update()
    {
        InternalUpdate();
    }

    private void FixedUpdate()
    {
        InternalFixedUpdate();
    }

    virtual protected void InternalAwake() { }
    virtual protected void InternalStart() { }
    virtual protected void InternalUpdate() { }
    virtual protected void InternalFixedUpdate() { }
}
