using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles rotation of a GameObject based on mouse movement.
/// Supports horizontal (Y-axis) and vertical (X-axis) rotation,
/// with configurable speed and angular limits.
/// 
/// Commonly used for player cameras or character look controls.
/// </summary>
public class InputRotator : MonoBehaviour
{
    /// <summary>
    /// Sensitivity multiplier applied to mouse movement.
    /// </summary>
    [SerializeField] float rotationSpeed = 0.1f;

    /// <summary>
    /// Enables or disables rotation based on horizontal mouse movement (X-axis).
    /// </summary>
    [Tooltip("Rotate the object according to the mouse X-axis movement?")]
    [SerializeField] bool horizontalRotation = true;

    /// <summary>
    /// Minimum allowed horizontal rotation angle (degrees).
    /// </summary>
    [SerializeField] float minHorizontalRotation = -360f;

    /// <summary>
    /// Maximum allowed horizontal rotation angle (degrees).
    /// </summary>
    [SerializeField] float maxHorizontalRotation = 360f;

    /// <summary>
    /// Enables or disables rotation based on vertical mouse movement (Y-axis).
    /// </summary>
    [Tooltip("Rotate the object according to the mouse Y-axis movement?")]
    [SerializeField] bool verticalRotation = true;

    /// <summary>
    /// Minimum allowed vertical rotation angle (degrees).
    /// </summary>
    [SerializeField] float minVerticalRotation = -45f;

    /// <summary>
    /// Maximum allowed vertical rotation angle (degrees).
    /// </summary>
    [SerializeField] float maxVerticalRotation = 45f;

    /// <summary>
    /// Input action used to read mouse delta movement.
    /// </summary>
    [SerializeField]
    InputAction lookLocation =
        new InputAction(type: InputActionType.Value);

    /// <summary>
    /// Enables the mouse look input action when this component is enabled.
    /// </summary>
    void OnEnable() { lookLocation.Enable(); }

    /// <summary>
    /// Disables the mouse look input action when this component is disabled.
    /// </summary>
    void OnDisable() { lookLocation.Disable(); }

    /// <summary>
    /// Ensures a default mouse delta binding exists for the look input action.
    /// Executed in the editor when values change.
    /// </summary>
    void OnValidate()
    {
        // Provide default bindings for the input action
        if (lookLocation.bindings.Count == 0)
            lookLocation.AddBinding("<Mouse>/delta");
    }

    /// <summary>
    /// Reads mouse movement each frame and applies
    /// constrained rotation to the GameObject.
    /// </summary>

    private float maxAngle = 360f;
    private float halfAngle = 180f;
    void Update()
    {
        Vector2 mouseDelta = lookLocation.ReadValue<Vector2>();
        Vector3 rotation = transform.localEulerAngles;

        // Horizontal rotation (Y-axis)
        if (horizontalRotation)
        {
            rotation.y = Mathf.Clamp(
                rotation.y + mouseDelta.x * rotationSpeed,
                minHorizontalRotation,
                maxHorizontalRotation
            );
        }

        // Vertical rotation (X-axis)
        if (verticalRotation)
        {
            float newRotationX = rotation.x - mouseDelta.y * rotationSpeed;

            // Normalize angle to [-180, 180] range
            if (newRotationX >maxAngle)
                newRotationX -= halfAngle;

            float clampedRotationX =
                Mathf.Clamp(newRotationX, minVerticalRotation, maxVerticalRotation);

            rotation.x = clampedRotationX;
        }

        transform.localEulerAngles = rotation;
    }
}
