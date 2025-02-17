using System;
using UnityEngine;

public class PlatformsParent : MonoBehaviour {
    public static PlatformsParent Instance { get; private set; }

    public event EventHandler OnPlatformsSwitched;

    [SerializeField] private SwitchablePlatformsParent whitePlatformsParent;
    [SerializeField] private SwitchablePlatformsParent blackPlatformsParent;

    private bool isWhiteSelected = true;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("A Player instance was already created.");
        }
        Instance = this;
    }

    private void Start() {
        GameInput.Instance.OnSwitchColorAction += GameInput_OnSwitchColorAction;

        whitePlatformsParent.Show();
        blackPlatformsParent.Hide();
    }

    private void GameInput_OnSwitchColorAction(object sender, EventArgs e) {
        if (GameManager.Instance.IsGameFinished() ||
            GameManager.Instance.IsGameOver() ||
            GameManager.Instance.IsPaused())
        {
            return;
        }

        isWhiteSelected = !isWhiteSelected;

        if (isWhiteSelected) {
            whitePlatformsParent.Show();
            blackPlatformsParent.Hide();
        } else {
            whitePlatformsParent.Hide();
            blackPlatformsParent.Show();
        }

        OnPlatformsSwitched?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy() {
        GameInput.Instance.OnSwitchColorAction -= GameInput_OnSwitchColorAction;
    }
}
