using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] Slider lifebar;

    public void SetMaxLife(float value)
    {
        lifebar.maxValue = value;
        lifebar.value = value;
    }

    public void UpdateLifebar(float value)
    {
        lifebar.value = value;
    }
}
