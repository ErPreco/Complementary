using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
/// </summary>
[SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class LockCameraZ : CinemachineExtension {
    [Tooltip("Lock the camera's Z position to this value")]
    [SerializeField] private float zPosition = -10;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if (stage == CinemachineCore.Stage.Body) {
            var pos = state.RawPosition;
            pos.z = zPosition;
            state.RawPosition = pos;
        }
    }

    public float GetZPosition() {
        return zPosition;
    }

    public void SetZPosition(float zPosition) {
        this.zPosition = zPosition;
    }
}
