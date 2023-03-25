using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    public GameObject Prefab => prefab;

    private Queue<GameObject> inactiveObjects = new();

    public GameObject GetObject()
    {
        if(inactiveObjects.Count > 0)
        {
            GameObject dequeuedObject = inactiveObjects.Dequeue();

            dequeuedObject.transform.parent = null;
            dequeuedObject.SetActive(true);

            IObjectPoolNotifier[] notifiers = dequeuedObject.GetComponents<IObjectPoolNotifier>();
            foreach (IObjectPoolNotifier item in notifiers)
            {
                item.OnCreatedOrDequeuedFromPool(false);
            }

            return dequeuedObject;
        }
        else
        {
            GameObject newObject = Instantiate(prefab);
            PooledObject poolTag = newObject.AddComponent<PooledObject>();
            poolTag.owner = this;

            poolTag.hideFlags = HideFlags.HideInInspector;

            IObjectPoolNotifier[] notifiers = newObject.GetComponents<IObjectPoolNotifier>();
            foreach (IObjectPoolNotifier item in notifiers)
            {
                item.OnCreatedOrDequeuedFromPool(true);
            }

            return newObject;
        }
    }

    public void ReturnObject(GameObject gameObject)
    {
        IObjectPoolNotifier[] notifiers = gameObject.GetComponents<IObjectPoolNotifier>();

        foreach (IObjectPoolNotifier item in notifiers)
        {
            item.OnEnqueuedToPool();
        }

        gameObject.SetActive(false);
        gameObject.transform.parent = this.transform;

        inactiveObjects.Enqueue(gameObject);
    }
}
