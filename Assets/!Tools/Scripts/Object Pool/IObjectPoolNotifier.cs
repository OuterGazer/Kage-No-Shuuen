using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectPoolNotifier
{
    public void OnEnqueuedToPool();

    public void OnCreatedOrDequeuedFromPool(bool isCreated, Transform patrolParent);
}
