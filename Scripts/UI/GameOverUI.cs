using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private void Awake() {
        restartButton.onClick.AddListener(() => {
            Loader.ReloadScene();
        });
        menuButton.onClick.AddListener(() => {
            Loader.LoadMenu();
        });
    }

    private void Start() {
        GameManager.Instance.OnRealtimeTimerEnded += GameManager_OnRealtimeTimerEnded;

        Hide();
    }

    private void GameManager_OnRealtimeTimerEnded(object sender, EventArgs e) {
        if (GameManager.Instance.IsGameOver()) {
            Show();
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
        GameManager.Instance.OnRealtimeTimerEnded -= GameManager_OnRealtimeTimerEnded;
    }
}
