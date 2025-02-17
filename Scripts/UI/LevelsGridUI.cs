using UnityEngine;

public class LevelsGridUI : MonoBehaviour {
    [SerializeField] private Transform levelButtonTemplate;

    private void Start() {
        levelButtonTemplate.gameObject.SetActive(false);

        UpdateVisual();
    }

    /// <summary>
    /// Updates the visual of the button.
    /// </summary>
    private void UpdateVisual() {
        foreach (Transform child in transform) {
            if (child == levelButtonTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= Loader.GetLevelsCount(); i++) {
            Transform levelButtonTansform = Instantiate(levelButtonTemplate, transform).transform;
            levelButtonTansform.gameObject.SetActive(true);
            
            LevelButtonUI levelButtonUI = levelButtonTansform.GetComponent<LevelButtonUI>();
            levelButtonUI.SetIndexText(i);
            
            int playerPrefsIsLevelLocked = PlayerPrefsManger.GetIsLevelLocked(i, i == 1 ? 0 : 1);   // by default levels are locked but the first
            bool isLevelLocked = i > 1 && playerPrefsIsLevelLocked > 0;   // the first level is always unlocked
            levelButtonUI.SetIsLevelLocked(isLevelLocked);

            int playerPrefsIsLevelFinished = PlayerPrefsManger.GetIsLevelFinished(i, 0);   // by default levels are unfinished
            bool isLevelFinished = playerPrefsIsLevelFinished > 0;
            levelButtonUI.SetIsLevelFinished(isLevelFinished);
        }
    }
}
