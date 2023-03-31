using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameSessionManager : Singleton<GameSessionManager>
{
    [SerializeField] Image fadeToBlack;
    [SerializeField] RectTransform pauseMenu;

    [SerializeField] InputAction pauseAction;

    private bool isGamePaused = false;
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.gameObject.SetActive(false);
        isGamePaused = false;
    }

    private void Awake()
    {
        fadeToBlack.DOFade(0f, 4f).SetEase(Ease.OutQuad).OnComplete(() => fadeToBlack.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    private void LateUpdate()
    {
        if (pauseAction.triggered)
        {
            if (!isGamePaused)
            {
                Time.timeScale = 0f;
                pauseMenu.gameObject.SetActive(true);
                isGamePaused = true;
            }
        }
    }
}
