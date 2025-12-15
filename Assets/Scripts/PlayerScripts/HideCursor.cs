using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the visibility and lock state of the mouse cursor.
/// Allows the player to toggle the cursor on and off using an input action.
/// 
/// By default, the cursor starts hidden and locked to the center of the screen,
/// which is typical for first-person or third-person gameplay.
/// </summary>
public class CursorHider : MonoBehaviour
{
    /// <summary>
    /// Input action used to toggle the cursor visibility.
    /// </summary>
    [SerializeField]
    InputAction toggleCursorAction =
        new InputAction(type: InputActionType.Button);

    /// <summary>
    /// Enables the input action when this component becomes active.
    /// </summary>
    void OnEnable() { toggleCursorAction.Enable(); }

    /// <summary>
    /// Disables the input action when this component becomes inactive.
    /// </summary>
    void OnDisable() { toggleCursorAction.Disable(); }

    /// <summary>
    /// Ensures a default input binding exists for the toggle action.
    /// This is executed in the editor when values change.
    /// </summary>
    void OnValidate()
    {
        // Provide default bindings for the input actions
        if (toggleCursorAction.bindings.Count == 0)
            toggleCursorAction.AddBinding("<Mouse>/leftButton");
    }

    /// <summary>
    /// Initializes the cursor state at game start.
    /// The cursor is hidden and locked.
    /// </summary>
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Checks each frame if the toggle input was performed
    /// and switches the cursor visibility and lock state accordingly.
    /// </summary>
    void Update()
    {
        if (toggleCursorAction.WasPerformedThisFrame())
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
