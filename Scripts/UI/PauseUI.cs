using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private void Awake() {
        resumeButton.onClick.AddListener(() => {
            GameManager.Instance.TogglePauseGame();
        });
        restartButton.onClick.AddListener(() => {
            Loader.ReloadScene();
        });
        menuButton.onClick.AddListener(() => {
            Loader.LoadMenu();
        });
    }

    private void Start() {
        GameManager.Instance.OnGamePauseChanged += GameManager_OnGamePauseChanged;

        Hide();
    }

    private void GameManager_OnGamePauseChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsPaused()) {
            Show();
        } else {
            Hide();
        }
    }

    /// <summary>
    /// Shows this GameObject.
    /// </summary>
    private void Show() {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides this GameObject.
    /// </summary>
    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        GameManager.Instance.OnGamePauseChanged -= GameManager_OnGamePauseChanged;
    }
}
