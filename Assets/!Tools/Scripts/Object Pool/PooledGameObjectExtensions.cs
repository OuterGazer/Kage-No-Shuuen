using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PooledGameObjectExtensions
{
    public static void ReturnToPool(this GameObject gameObject)
    {
        PooledObject pooledObject = gameObject.GetComponent<PooledObject>();

        if (!pooledObject)
        {
            Debug.LogErrorFormat(gameObject, $"Cannot return {gameObject} to object pool because it was not created from one.");
            return;
        }

        pooledObject.owner.ReturnObject(gameObject);
    }
}