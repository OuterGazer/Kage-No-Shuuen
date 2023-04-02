using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] RectTransform mainMenu;
    [SerializeField] Image loadingImage;
    [SerializeField] TextMeshProUGUI briefingText;
    [SerializeField] Slider loadingBar;
    [SerializeField] TextMeshProUGUI pressAnyKeyText;
    [SerializeField] CanvasGroup fadeToBlack;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Time.timeScale < 1f)
        { Time.timeScale = 1; }
    }

    public void StartNewGame()
    {
        mainMenu?.DOAnchorPosX(-400f, 1f).SetEase(Ease.OutQuart).OnComplete(() => StartCoroutine(PerformLoadingScreen()));
    }

    public void GoToMenu(RectTransform inMenu)
    {
        mainMenu.DOAnchorPosX(-400f, 1f).SetEase(Ease.OutQuart);
        inMenu.DOAnchorPosX(0f, 1f).SetEase(Ease.OutQuart);
    }

    public void GoBackToMainMenu(RectTransform inMenu)
    {
        mainMenu.DOAnchorPosX(500f, 1f).SetEase(Ease.OutQuart);
        inMenu.DOAnchorPosX(1500, 1f).SetEase(Ease.OutQuart);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator PerformLoadingScreen()
    {
        loadingImage.gameObject.SetActive(true);

        loadingImage.DOFade(255f, 0.75f).SetEase(Ease.OutQuart)
            .OnComplete(() => loadingBar.gameObject.SetActive(true));        

        yield return new WaitUntil(() => loadingBar.gameObject.activeInHierarchy);

        loadingBar.DOValue(50f, 4f).SetEase(Ease.Linear);

        yield return new WaitUntil(() => loadingBar.value > 49.85f);

        yield return new WaitForSeconds(1f);

        loadingBar.value = 66f;

        yield return new WaitForSeconds(0.5f);

        loadingBar.DOValue(90f, 0.5f).SetEase(Ease.InExpo)
            .OnComplete(() => loadingBar.DOValue(100f, 1f).SetEase(Ease.Linear));

        yield return new WaitUntil(() => Mathf.Approximately(loadingBar.value, 100f));

        pressAnyKeyText.gameObject.SetActive(true);

        pressAnyKeyText.DOFade(0f, 1f).SetLoops(-1, LoopType.Yoyo);

        yield return new WaitUntil(() => Keyboard.current.anyKey.isPressed);

        fadeToBlack.DOFade(1f, 2f).SetEase(Ease.Linear).OnComplete(LoadLevel);
    }

    private void LoadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }
}
