using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelFinishedUI : MonoBehaviour {
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private void Awake() {
        nextLevelButton.onClick.AddListener(() => {
            Loader.LoadNextLevel();
        });
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
        if (GameManager.Instance.IsGameFinished()) {
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
