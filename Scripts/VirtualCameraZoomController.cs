using UnityEngine;

public class VirtualCameraZoomController : MonoBehaviour {
    [SerializeField] private float minZoom = -20;
    [SerializeField] private float maxZoom = -10;
    [SerializeField] private float scrollStep = 1;
    [SerializeField] private float zoomSpeed = 5;

    private LockCameraZ lockCameraZ;
    private float newZPosition;

    private void Awake() {
        lockCameraZ = GetComponent<LockCameraZ>();
    }

    private void Start() {
        GameInput.Instance.OnZoomAction += GameInput_OnZoomAction;

        newZPosition = lockCameraZ.GetZPosition();
    }

    private void LateUpdate() {
        float zPosition = lockCameraZ.GetZPosition();
        zPosition = Mathf.Lerp(zPosition, newZPosition, Time.deltaTime * zoomSpeed);
        lockCameraZ.SetZPosition(zPosition);
    }

    private void GameInput_OnZoomAction(object sender, GameInput.OnZoomActionEventArgs e) {
        newZPosition += e.normalizedScroll * scrollStep;
        newZPosition = Mathf.Clamp(newZPosition, minZoom, maxZoom);
    }

    private void OnDestroy() {
        GameInput.Instance.OnZoomAction -= GameInput_OnZoomAction;
    }
}
