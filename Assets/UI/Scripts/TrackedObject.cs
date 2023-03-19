using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObject : MonoBehaviour
{
    private void Start()
    {
        IndicatorManager.manager.AddTrackingIndicator(this);
    }

    private void OnDestroy()
    {
        IndicatorManager.manager.RemoveTrackingIndicator(this);
    }
}
