using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTweener : MonoBehaviour
{
    public enum TweeningType
    {
        localMove,
        localRotate,
        localScale,
    }

    [SerializeField] Transform objectToTween;
    [SerializeField] Vector3 targetLocalVector;
    [SerializeField] float tweeningTime = 1f;

    [SerializeField] TweeningType tweeningType = TweeningType.localMove;
    [SerializeField] Ease easingFunction = Ease.Linear;

    [SerializeField] AudioClip soundToPlay1;
    [SerializeField] AudioClip soundToPlay2;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PerformTweening()
    {
        switch (tweeningType)
        {
            case TweeningType.localMove:
                audioSource.PlayOneShot(soundToPlay1);
                objectToTween.DOLocalMove(targetLocalVector, tweeningTime).SetEase(easingFunction).OnComplete(() => audioSource.PlayOneShot(soundToPlay2));

                break;
            case TweeningType.localRotate:
                objectToTween.DOLocalRotate(targetLocalVector, tweeningTime).SetEase(easingFunction);
                break;
            case TweeningType.localScale:
                objectToTween.DOScale(targetLocalVector, tweeningTime).SetEase(easingFunction);
                break;
        }
    }
}
