using System;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {
    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    private int previousCountdownNumber;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Show();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsCountDownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (countdownNumber != previousCountdownNumber) {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP);
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
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }
}
