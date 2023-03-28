using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] Image hoverOverImage;
    [SerializeField] Image buttonImage;

    private Button button;

    public void OnPointerEnter()
    {
        hoverOverImage.gameObject.SetActive(true);
        if (buttonImage) { buttonImage.enabled = false; }
    }

    public void OnPointerExit()
    {
        hoverOverImage.gameObject.SetActive(false);
        if (buttonImage) { buttonImage.enabled = true; }
    }

    public void OnPointerClick()
    {
        hoverOverImage.gameObject.SetActive(false);
        if (buttonImage) { buttonImage.enabled = true; }
    }
}
