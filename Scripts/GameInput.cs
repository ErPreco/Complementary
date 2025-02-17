using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpAction;
    public event EventHandler OnSwitchColorAction;
    public event EventHandler<OnZoomActionEventArgs> OnZoomAction;
    public class OnZoomActionEventArgs : EventArgs {
        public float normalizedScroll;
    }
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("A GameInput instance was already created.");
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_Performed;
        playerInputActions.Player.SwitchColor.performed += SwitchColor_Performed;
        playerInputActions.Player.Zoom.performed += Zoom_Performed;
        playerInputActions.Player.Pause.performed += Pause_Performed;
    }

    private void Jump_Performed(InputAction.CallbackContext action) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void SwitchColor_Performed(InputAction.CallbackContext action) {
        OnSwitchColorAction?.Invoke(this, EventArgs.Empty);
    }

    private void Zoom_Performed(InputAction.CallbackContext action) {
        float normalizedScroll = Mathf.Sign(action.ReadValue<float>());
        OnZoomAction?.Invoke(this, new OnZoomActionEventArgs {
            normalizedScroll = normalizedScroll
        });
    }

    private void Pause_Performed(InputAction.CallbackContext action) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns the direction of the movement.
    /// </summary>
    /// <returns>One value between {-1, 0, 1}.</returns>
    public int GetMovementDirection() {
        return (int)playerInputActions.Player.Move.ReadValue<float>();
    }

    private void OnDestroy() {
        playerInputActions.Player.Jump.performed -= Jump_Performed;
        playerInputActions.Player.SwitchColor.performed -= SwitchColor_Performed;
        playerInputActions.Player.Zoom.performed -= Zoom_Performed;
    }
}
