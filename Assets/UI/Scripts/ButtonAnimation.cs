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

    [Header("SFX Settings")]
    [SerializeField] AudioClip hoverOverMainMenu;
    [SerializeField] AudioClip hoverOverPauseMenu;
    [SerializeField] AudioClip confirm;

    private AudioSource audioSource;
    private RectTransform rectTransform;
    private float originalLocalPositionY;

    private void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
        rectTransform = GetComponent<RectTransform>();
        originalLocalPositionY = rectTransform.anchoredPosition.y;
    }

    public void OnPointerEnter()
    {
        audioSource.PlayOneShot(hoverOverMainMenu);
        hoverOverImage.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        hoverOverImage.gameObject.SetActive(false);
    }

    public void OnPointerClick()
    {
        audioSource.PlayOneShot(confirm);
        hoverOverImage.gameObject.SetActive(false);
    }

    public void OnPointerEnterPauseMenu()
    {
        audioSource.PlayOneShot(hoverOverPauseMenu);

        rectTransform.DOAnchorPosY((originalLocalPositionY - yAnimationDistance), timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerExitPauseMenu()
    {
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerClickPauseMenu()
    {
        audioSource.PlayOneShot(confirm);
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }
}
