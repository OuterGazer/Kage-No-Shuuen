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
        //if (buttonImage) { buttonImage.enabled = false; }
    }

    public void OnPointerExit()
    {
        hoverOverImage.gameObject.SetActive(false);
        //if (buttonImage) { buttonImage.enabled = true; }
    }

    public void OnPointerClick()
    {
        audioSource.PlayOneShot(confirm);
        hoverOverImage.gameObject.SetActive(false);
        //if (buttonImage) { buttonImage.enabled = true; }
    }

    public void OnPointerEnterPauseMenu()
    {
        Time.timeScale = 1.0f;
        audioSource.PlayOneShot(hoverOverPauseMenu);
        Time.timeScale = 0f;

        rectTransform.DOAnchorPosY((originalLocalPositionY - yAnimationDistance), timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerExitPauseMenu()
    {
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }

    public void OnPointerClickPauseMenu()
    {
        Time.timeScale = 1f;
        audioSource.PlayOneShot(confirm);
        Time.timeScale = 0f;
        rectTransform.DOAnchorPosY(originalLocalPositionY, timeToCompleteAnimation).SetEase(ease).SetUpdate(true);
    }
}
