using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameSessionManager : Singleton<GameSessionManager>
{
    [SerializeField] Image fadeToBlack;
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] CanvasGroup youAreDeadText;
    [SerializeField] Vector3 initialPosition;
    [SerializeField] Vector3 checkpointPosition;

    [SerializeField] InputAction pauseAction;

    private bool isGamePaused = false;
    private bool isChekpointActive = false;
    public void ActivateCheckpoint()
    {
        isChekpointActive = true;
    }

    public bool debugDeath = false;

    private void Update()
    {
        if (debugDeath)
        {
            GameObject.FindWithTag("Player").GetComponentInChildren<DamageableWithLife>().ReceiveDamage(20f);
            debugDeath = false;
        }
    }

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
                pauseMenu.DOScale(1f, 0.2f).From(0f, true).SetEase(Ease.OutBack).SetUpdate(true);
                isGamePaused = true;
            }
        }
    }

    public void RestartGame()
    {
        fadeToBlack.gameObject.SetActive(true);

        StartCoroutine(PlayDeathFanciness());
    }

    private IEnumerator PlayDeathFanciness()
    {
        yield return new WaitForSeconds(2.6f);

        youAreDeadText.DOFade(1f, 6f).SetEase(Ease.OutSine)
            .OnComplete(() => fadeToBlack.DOFade(1f, 1f).SetEase(Ease.OutSine))
            .OnComplete(() => youAreDeadText.alpha = 0f);

        yield return new WaitForSeconds(7f);

        Transform player = FindObjectOfType<StateController>().transform;

        player.gameObject.SetActive(false);

        player.position = !isChekpointActive ? initialPosition : checkpointPosition;

        yield return new WaitForEndOfFrame();

        player.gameObject.SetActive(true);

        fadeToBlack.DOFade(0f, 2f).SetEase(Ease.OutSine);
    }
}
