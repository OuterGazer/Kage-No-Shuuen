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
    [SerializeField] Image fadeToBlack;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransform.DOScale(0f, 0.35f).From().SetEase(Ease.OutElastic);
    }

    public void ReturnToGame()
    {
        GameSessionManager.Instance.ResumeGame();
    }

    public void GoToMenu(RectTransform inMenu)
    {
        pauseMenu.DOAnchorPosX(-400f, 1f).SetEase(Ease.OutQuart);
        inMenu.DOAnchorPosX(0f, 1f).SetEase(Ease.OutQuart);
    }

    public void GoBackToPauseMenu(RectTransform inMenu)
    {
        pauseMenu.DOAnchorPosX(500f, 1f).SetEase(Ease.OutQuart);
        inMenu.DOAnchorPosX(1500, 1f).SetEase(Ease.OutQuart);
    }

    public void ExitToMainMenu()
    {
        fadeToBlack.gameObject.SetActive(true);
        fadeToBlack.DOFade(255f, 2f).SetUpdate(true).OnComplete(() => SceneManager.LoadScene(0));
    }
}
