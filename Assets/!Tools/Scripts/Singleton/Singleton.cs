using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindOrCreateInstance();
            }

            return instance;
        }
    }

    private static T instance;

    private static T FindOrCreateInstance()
    {
        var instance = GameObject.FindObjectOfType<T>();

        if (instance != null) 
        { return instance; }

        string name = typeof(T).Name + " Singleton";
        var containerGameObject = new GameObject(name);

        var singletonComponent = containerGameObject.AddComponent<T>();

        return singletonComponent;
    }     
}
