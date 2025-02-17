using UnityEngine;

public class SwitchablePlatformsParent : MonoBehaviour {
    /// <summary>
    /// Activates all children of the transform.
    /// </summary>
    public void Show() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivates all children of the platform.
    /// </summary>
    public void Hide() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }
}
