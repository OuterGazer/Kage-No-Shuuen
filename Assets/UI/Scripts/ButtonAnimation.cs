using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour
{
    [Header("Main Menu Button Settings")]
    [SerializeField] Image hoverOverImage;
    [SerializeField] Image buttonImage;

    [Header("Pause Menu Button Settings")]
    [SerializeField] float yAnimationDistance = 1.0f;
    [SerializeField] float timeToCompleteAnimation = 0.25f;
    [SerializeField] Ease ease;

    private RectTransform rectTransform;
    private float originalLocalPositionY;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalLocalPositionY = rectTransform.anchoredPosition.y;
    }

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

    public void OnPointerEnterPauseMenu()
    {
        rectTransform.DOAnchorPosY((originalLocalPositionY - yAnimationDistance), timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerExitPauseMenu()
    {
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerClickPauseMenu()
    {
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }
}
