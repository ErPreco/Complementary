using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelIndexText;
    [SerializeField] private GameObject lockedImage;
    [SerializeField] private Color levelFinishedOutlineColor;

    /// <summary>
    /// Sets the text of the button with the given index of the level.
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    public void SetIndexText(int levelIndex) {
        levelIndexText.text = levelIndex.ToString();
    }

    /// <summary>
    /// Links the click event to loading the corresponding level if it is unlocked, otherwise darkens the button.
    /// </summary>
    /// <param name="isLocked">Whether the level is locked or not.</param>
    public void SetIsLevelLocked(bool isLocked) {
        lockedImage.SetActive(isLocked);
        if (!isLocked) {
            GetComponent<Button>().onClick.AddListener(() => {
                Loader.LoadLevelByIndex(int.Parse(levelIndexText.text) - 1);
            });
        }
    }

    /// <summary>
    /// Chenges the outline of the button whether the level is finished or not.
    /// </summary>
    /// <param name="isFinished">Whether the level is finished or not.</param>
    public void SetIsLevelFinished(bool isFinished) {
        Outline outline = GetComponent<Outline>();
        Vector2 effectDistance = new Vector2(3, 3);
        Color effectColor = Color.black;
        if (isFinished) {
            effectDistance = new Vector2(5, 5);
            effectColor = levelFinishedOutlineColor;
        }
        
        outline.effectColor = effectColor;
        outline.effectDistance = effectDistance;
    }
}
