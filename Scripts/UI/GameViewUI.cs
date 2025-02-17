using TMPro;
using UnityEngine;

public class GameViewUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    
    private float timer;

    private void Start() {
        timer = GameManager.Instance.GetLevelTime();
        SetTimerText();
    }

    private void Update() {
        if (GameManager.Instance.IsGamePlaying()) {
            timer -= Time.deltaTime;
            SetTimerText();
        }
    }

    /// <summary>
    /// Sets the timer text.
    /// </summary>
    private void SetTimerText() {
        int minutes = Mathf.CeilToInt(timer) / 60;
        int seconds = Mathf.CeilToInt(timer) % 60;
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
