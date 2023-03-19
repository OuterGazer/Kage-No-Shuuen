using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField] RectTransform indicatorPrefab = null;
    [SerializeField] RectTransform indicatorContainer = null;
    
    public static IndicatorManager manager;

    Dictionary<TrackedObject, RectTransform> indicators = new();

    private void Awake()
    {
        manager = this;
    }

    private void LateUpdate()
    {
        foreach(KeyValuePair<TrackedObject, RectTransform> pair in indicators)
        {
            TrackedObject target = pair.Key;
            RectTransform indicator = pair.Value;

            if (!target) { continue; }

            indicator.anchoredPosition = GetCanvasPositionForTarget(target);
        }
    }

    private Vector2 GetCanvasPositionForTarget(TrackedObject target)
    {
        Vector3 indicatorPoint = Camera.main.WorldToViewportPoint(target.transform.position);

        indicatorPoint.x = Mathf.Clamp01(indicatorPoint.x);
        indicatorPoint.y = Mathf.Clamp01(indicatorPoint.y);

        if(indicatorPoint.z < 0f) 
        { 
            indicatorPoint.y = 0f; 
            indicatorPoint.x = 1f - indicatorPoint.x;
        }

        Canvas canvas = indicatorContainer.GetComponentInParent<Canvas>();
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

        indicatorPoint.Scale(canvasSize);

        return indicatorPoint;
    }

    public void AddTrackingIndicator(TrackedObject transform)
    {
        if (indicators.ContainsKey(transform)) { return; }

        RectTransform indicator = Instantiate(indicatorPrefab);
        indicator.name = string.Format($"Indicator for {transform.gameObject.name}");

        indicator.SetParent(indicatorContainer, false);

        indicator.pivot = new Vector2(0.5f, 0.5f);

        indicator.anchorMin = Vector2.zero;
        indicator.anchorMax = Vector2.zero;

        indicators[transform] = indicator;

        indicator.anchoredPosition = GetCanvasPositionForTarget(transform);
    }

    public void RemoveTrackingIndicator(TrackedObject transform)
    {
        if (indicators.ContainsKey(transform))
        {
            if (indicators[transform])
            {
                Destroy(indicators[transform].gameObject);
            }
        }

        indicators.Remove(transform);
    }
}
