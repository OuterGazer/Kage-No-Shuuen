using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] RectTransform optionsMenu;
    [SerializeField] CanvasGroup fadeToBlack;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ReturnToGame()
    {
        GameSessionManager.Instance.HideMousePointer();
        GameSessionManager.Instance.ResumeGame();
    }

    public void GoToMenu(RectTransform inMenu)
    {
        inMenu.gameObject.SetActive(true);

        pauseMenu.DOScale(0f, 0.2f).From(1f, true).SetEase(Ease.OutCubic).SetUpdate(true)
            .OnComplete(() => inMenu.DOScale(1f, 0.2f).From(0f, true).SetEase(Ease.OutBack).SetUpdate(true));
    }

    public void GoBackToPauseMenu(RectTransform inMenu)
    {
        inMenu.DOScale(0f, 0.2f).From(1f, true).SetEase(Ease.OutCubic).SetUpdate(true)
            .OnComplete(() => pauseMenu.DOScale(1f, 0.2f).From(0f, true).SetEase(Ease.OutBack).SetUpdate(true));
    }

    public void ExitToMainMenu()
    {
        //fadeToBlack.gameObject.SetActive(true);
        fadeToBlack.DOFade(1f, 2f).SetUpdate(true).OnComplete(() => SceneManager.LoadScene(0));
    }
}
