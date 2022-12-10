using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHook : MonoBehaviour
{
    public UnityEvent onHookLaunched;


    void OnHookThrow()
    {
        onHookLaunched.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
