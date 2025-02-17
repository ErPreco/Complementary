using UnityEngine;

public class PlayerPrefsManger : MonoBehaviour {
    private static string IS_LEVEL_LOCKED { get { return "isLevelLocked"; } }
    private static string IS_LEVEL_FINISHED { get { return "isLevelFinished"; } }

    [Tooltip("Only for development purposes. Toggle on only when you are sure to delete every PlayerPrefs keys.")]
    [SerializeField] private bool deleteAllKeys;

    private void Awake() {
        if (deleteAllKeys) {
            PlayerPrefs.DeleteAll();
        }
    }

    /// <summary>
    /// Returns the "isLevelLocked" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <returns>The "isLevelLocked" value or 0 if the variable does not exists (0 = unlocked, 1 = locked).</returns>
    public static int GetIsLevelLocked(int levelIndex) {
        return PlayerPrefs.GetInt(IS_LEVEL_LOCKED + levelIndex);
    }

    /// <summary>
    /// Returns the "isLevelLocked" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The "isLevelLocked" value or the default value given if the variable does not exists (0 = unlocked, 1 = locked).</returns>
    public static int GetIsLevelLocked(int levelIndex, int defaultValue) {
        return PlayerPrefs.GetInt(IS_LEVEL_LOCKED + levelIndex, defaultValue);
    }

    /// <summary>
    /// Returns the "isLevelFinished" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <returns>The "isLevelFinished" value or 0 if the variable does not exists (0 = unfinished, 1 = finished).</returns>
    public static int GetIsLevelFinished(int levelIndex) {
        return PlayerPrefs.GetInt(IS_LEVEL_FINISHED + levelIndex);
    }

    /// <summary>
    /// Returns the "isLevelFinished" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The "isLevelFinished" value or the default value given if the variable does not exists (0 = unfinished, 1 = finished).</returns>
    public static int GetIsLevelFinished(int levelIndex, int defaultValue) {
        return PlayerPrefs.GetInt(IS_LEVEL_FINISHED + levelIndex, defaultValue);
    }

    /// <summary>
    /// Sets the "isLevelLocked" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <param name="value">The new value to set (0 = unlocked, 1 = locked).</param>
    public static void SetIsLevelLocked(int levelIndex, int value) {
        PlayerPrefs.SetInt(IS_LEVEL_LOCKED + levelIndex, value);
    }

    /// <summary>
    /// Sets the "isLevelFinished" value of the i-th given level (i starts from 1 for the first level).
    /// </summary>
    /// <param name="levelIndex">The index of the level.</param>
    /// <param name="value">The new value to set (0 = unfinished, 1 = finished).</param>
    public static void SetIsLevelFinished(int levelIndex, int value) {
        PlayerPrefs.SetInt(IS_LEVEL_FINISHED + levelIndex, value);
    }
}
