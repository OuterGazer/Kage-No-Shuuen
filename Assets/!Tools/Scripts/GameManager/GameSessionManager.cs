using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameSessionManager : Singleton<GameSessionManager>
{
    [SerializeField] CanvasGroup fadeToBlack;
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

#if UNITY_EDITOR
    public bool debugDeath = false;
#endif

    private void Update()
    {
#if UNITY_EDITOR
        if (debugDeath)
        {
            GameObject.FindWithTag("Player").GetComponent<DamageableWithLife>().ReceiveDamage(20f);
            debugDeath = false;
        }
#endif
    }

    public void HideMousePointer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowMousePointer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.gameObject.SetActive(false);
        isGamePaused = false;
    }

    private void Awake()
    {
        fadeToBlack.DOFade(0f, 6f).SetEase(Ease.OutQuad);//.OnComplete(() => fadeToBlack.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        pauseAction?.Enable();
        HideMousePointer();
        SoundManager.Instance.ChangeToGameTrack();
    }

    private void OnDisable()
    {
        pauseAction?.Disable();
    }

    private void LateUpdate()
    {
        if (pauseAction != null && pauseAction.triggered)
        {
            if (!isGamePaused)
            {
                Time.timeScale = 0f;
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.DOScale(1f, 0.2f).From(0f, true).SetEase(Ease.OutBack).SetUpdate(true);
                isGamePaused = true;
                GameSessionManager.Instance.ShowMousePointer();
                SoundManager.Instance.PauseMusic();
            }
        }
    }

    public void RestartGame()
    {
        //fadeToBlack.gameObject.SetActive(true);

        StartCoroutine(PlayDeathFanciness());
    }

    private IEnumerator PlayDeathFanciness()
    {
        yield return new WaitForSeconds(2.6f);

        youAreDeadText.DOFade(1f, 6f).SetEase(Ease.OutSine);
            
        yield return new WaitForSeconds(6f);

        fadeToBlack.DOFade(1f, 3f).SetEase(Ease.OutSine)
            .OnComplete(() => youAreDeadText.alpha = 0f);

        yield return new WaitForSeconds(3f);

        Transform player = FindObjectOfType<StateController>()?.transform;

        player.gameObject.SetActive(false);

        player.position = !isChekpointActive ? initialPosition : checkpointPosition;

        yield return new WaitForEndOfFrame();

        player.gameObject.SetActive(true);

        fadeToBlack.DOFade(0f, 2f).SetEase(Ease.OutSine);
    }
}
