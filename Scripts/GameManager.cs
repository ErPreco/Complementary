using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnRealtimeTimerEnded;
    public event EventHandler OnGamePauseChanged;

    private enum State {
        CountdownToStart,
        GamePlaying,
        GameOver,
        GameFinished
    }

    [SerializeField] private int levelTime;

    private int countdownToStartTime = 3;
    private float deathTime = 2;
    private float levelFinishedTime = 1;

    private State state;
    private float timer;
    private bool isPause;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("A GameManager instance was already created.");
        }
        Instance = this;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;

        state = State.CountdownToStart;
        timer = countdownToStartTime;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        if (state == State.GameOver || state == State.GameFinished) return;
        TogglePauseGame();
    }

    private void Update() {
        switch (state) {
            case State.CountdownToStart:
                timer -= Time.deltaTime;
                if (timer < 0) {
                    timer = 0;
                    PlayGame();
                }
                break;
            case State.GamePlaying:
                timer += Time.deltaTime;
                if (timer > levelTime) {
                    timer = 0;
                    GameOver();
                }
                break;
            case State.GameOver:
                break;
            case State.GameFinished:
                break;
        }
    }

    /// <summary>
    /// Returns the timer if the game is on the countdown state, 0 otherwise.
    /// </summary>
    /// <returns>The timer if the game is on the countdown state, 0 otherwise.</returns>
    public float GetCountdownToStartTimer() {
        return state == State.CountdownToStart ? timer : 0;
    }

    /// <summary>
    /// Returns whether the game is on countdown state or not.
    /// </summary>
    /// <returns>Whether the game is on countdown state or not.</returns>
    public bool IsCountDownToStartActive() {
        return state == State.CountdownToStart;
    }

    /// <summary>
    /// Returns whether the game is playing or not.
    /// </summary>
    /// <returns>Whether the game is playing or not.</returns>
    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    /// <summary>
    /// Returns whether the game is over or not.
    /// </summary>
    /// <returns>Whether the game is over or not.</returns>
    public bool IsGameOver() {
        return state == State.GameOver;
    }

    /// <summary>
    /// Returns whether the game is finished or not.
    /// </summary>
    /// <returns>Whether the game is finished or not.</returns>
    public bool IsGameFinished() {
        return state == State.GameFinished;
    }

    /// <summary>
    /// Returns whether the game is paused or not.
    /// </summary>
    /// <returns>Whether the game is paused or not.</returns>
    public bool IsPaused() {
        return isPause;
    }

    /// <summary>
    /// Returns the time in seconds of the level.
    /// </summary>
    /// <returns>The time in seconds of the level.</returns>
    public int GetLevelTime() {
        return levelTime;
    }

    /// <summary>
    /// Changes the game state to GamePlaying.
    /// </summary>
    private void PlayGame() {
        state = State.GamePlaying;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Changes the game state to GameOver.
    /// </summary>
    public async void GameOver() {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);

        await RealtimeTimer(deathTime);
    }

    /// <summary>
    /// Changes the game state to GameFinished.
    /// </summary>
    public async void GameFinished() {
        state = State.GameFinished;
        OnStateChanged?.Invoke(this, EventArgs.Empty);

        int currentLevelIndex = Loader.GetCurrentLevelIndex();
        PlayerPrefsManger.SetIsLevelFinished(currentLevelIndex + 1, 1);
        if (currentLevelIndex + 1 < Loader.GetLevelsCount()) {
            PlayerPrefsManger.SetIsLevelLocked(currentLevelIndex + 2, 0);
        }

        await RealtimeTimer(levelFinishedTime);
    }

    /// <summary>
    /// Sets an async timer using unscaled time.
    /// </summary>
    /// <param name="timer">The timer in seconds.</param>
    private async Task RealtimeTimer(float timer) {
        int timerInMillisenconds = (int)timer * 1000;
        await Task.Delay(timerInMillisenconds);
        
        OnRealtimeTimerEnded?.Invoke(this, EventArgs.Empty);
    }

    #region WebGL
    // Work around to maintain the "delay" functionality for WebGL

    /*
    public void GameOver() {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);

        StartCoroutine(RealtimeTimerCO(deathTime));
    }

    public void GameFinished() {
        state = State.GameFinished;
        OnStateChanged?.Invoke(this, EventArgs.Empty);

        int currentLevelIndex = Loader.GetCurrentLevelIndex();
        PlayerPrefsManger.SetIsLevelFinished(currentLevelIndex + 1, 1);
        if (currentLevelIndex + 1 < Loader.GetLevelsCount()) {
            PlayerPrefsManger.SetIsLevelLocked(currentLevelIndex + 2, 0);
        }

        StartCoroutine(RealtimeTimerCO(levelFinishedTime));
    }

    private System.Collections.IEnumerator RealtimeTimerCO(float timer) {
        yield return new WaitForSecondsRealtime(timer);

        OnRealtimeTimerEnded?.Invoke(this, EventArgs.Empty);
    }
    */
    #endregion

    /// <summary>
    /// Toggles the pause of the game.
    /// </summary>
    public void TogglePauseGame() {
        isPause = !isPause;
        Time.timeScale = isPause ? 0 : 1;
        OnGamePauseChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        GameInput.Instance.OnPauseAction -= GameInput_OnPauseAction;
    }
}
