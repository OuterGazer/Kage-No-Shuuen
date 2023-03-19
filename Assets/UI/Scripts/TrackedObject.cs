using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObject : MonoBehaviour
{
    [SerializeField] RectTransform indicatorPrefab = null;
    public RectTransform IndicatorPrefab => indicatorPrefab;

    private bool isIndicatorVisible = false;
    public bool IsIndicatorVisible => isIndicatorVisible;
    public void SetIsIndicatorVisible(bool isIndicatorVisible) => this.isIndicatorVisible = isIndicatorVisible;

    private void Start()
    {
        IndicatorManager.manager.AddTrackingIndicator(this);
        isIndicatorVisible = false;
    }

    private void OnDestroy()
    {
        IndicatorManager.manager.RemoveTrackingIndicator(this);
    }
}
