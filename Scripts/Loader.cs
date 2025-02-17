using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {
    private static Loader Instance;

    private static string targetSceneName;
    private static int currentLevelIndex = -1;

    [SerializeField] private string menuSceneName;
    [SerializeField] private string howToPlaySceneName;
    [SerializeField] private string loadingSceneName;
    [SerializeField] private LevelSceneListSO levelSceneListSO;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("A Loader instance was already created.");
        }
        Instance = this;
    }

    /// <summary>
    /// Returns the current level index.
    /// </summary>
    /// <returns>The current level index.</returns>
    public static int GetCurrentLevelIndex() {
        return currentLevelIndex;
    }

    /// <summary>
    /// Returns the number of levels.
    /// </summary>
    /// <returns>The number of levels.</returns>
    public static int GetLevelsCount() {
        return Instance.levelSceneListSO.levelSceneNameList.Count;
    }

    /// <summary>
    /// Loads the MenuScene scene.
    /// </summary>
    public static void LoadMenu() {
        currentLevelIndex = -1;
        LoadTargetScene(Instance.menuSceneName);
    }

    /// <summary>
    /// Loads the HowToPlayScene scene.
    /// </summary>
    public static void LoadHowToPlay() {
        currentLevelIndex = -1;
        LoadTargetScene(Instance.howToPlaySceneName);
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public static void ReloadScene() {
        LoadScene(Instance.loadingSceneName);
    }

    /// <summary>
    /// Loads the level by the corresponding index of the LevelSceneListSO list.
    /// </summary>
    /// <param name="levelIndex">The index of the level in the LevelSceneListSO list (X if the scene name is LevelScene_X).</param>
    public static void LoadLevelByIndex(int levelIndex) {
        currentLevelIndex = levelIndex;
        LoadTargetScene(Instance.levelSceneListSO.levelSceneNameList[levelIndex]);
    }

    /// <summary>
    /// Loads the next level according to the LevelSceneListSO list. Goes back to the menu if the current level is the last one.
    /// </summary>
    public static void LoadNextLevel() {
        if (currentLevelIndex + 1 == Instance.levelSceneListSO.levelSceneNameList.Count) {
            LoadMenu();
            return;
        }

        currentLevelIndex++;
        LoadTargetScene(Instance.levelSceneListSO.levelSceneNameList[currentLevelIndex]);
    }

    /// <summary>
    /// Loads the actual target scene.
    /// </summary>
    public static void LoaderCallback() {
        LoadScene(targetSceneName);
    }

    /// <summary>
    /// Sets the target scene and loads the LoadingScene scene.
    /// </summary>
    /// <param name="targetScene">The scene to load.</param>
    private static void LoadTargetScene(string targetScene) {
        targetSceneName = targetScene;
        LoadScene(Instance.loadingSceneName);
    }

    /// <summary>
    /// Loads the scene with corresponding to the given name.
    /// </summary>
    /// <param name="sceneName">The scene name.</param>
    private static void LoadScene(string sceneName) {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
