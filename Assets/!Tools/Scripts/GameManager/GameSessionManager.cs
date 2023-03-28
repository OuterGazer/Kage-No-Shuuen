using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField] Image fadeToBlack;

    private void Awake()
    {
        fadeToBlack.DOFade(0f, 4f).SetEase(Ease.OutQuad).OnComplete(() => fadeToBlack.gameObject.SetActive(false));
    }
}
