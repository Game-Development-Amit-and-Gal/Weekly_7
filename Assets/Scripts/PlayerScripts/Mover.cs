using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles basic player movement using the keyboard and a CharacterController.
/// Supports:
/// - walking (WASD),
/// - sprinting (Shift),
/// - crouching (C / Ctrl),
/// - jumping (Space),
/// - gravity handling.
/// 
/// Movement is performed in local space and transformed to world space.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimpleKeyboardMover : MonoBehaviour
{
    // ---------------- Movement ----------------

    /// <summary>
    /// Movement speed while walking.
    /// </summary>
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;

    /// <summary>
    /// Movement speed while crouching.
    /// </summary>
    [SerializeField] private float crouchSpeed = 2.5f;

    /// <summary>
    /// Movement speed while sprinting.
    /// </summary>
    [SerializeField] private float sprintSpeed = 2.5f;

    /// <summary>
    /// Maximum duration the player can sprint.
    /// </summary>
    private float sprintTime = 4f;

    /// <summary>
    /// Countdown timer for remaining sprint time.
    /// </summary>
    private float countDown = 4f;

    // ---------------- Jump & Gravity ----------------

    /// <summary>
    /// Gravity acceleration applied while airborne.
    /// </summary>
    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;

    /// <summary>
    /// Initial upward velocity applied when jumping.
    /// </summary>
    [SerializeField] private float jumpVelocity = 6f;

    /// <summary>
    /// Small downward force applied while grounded
    /// to keep the character firmly on the ground.
    /// </summary>
    [SerializeField] private float groundStickY = -1f;

    // ---------------- Collider settings ----------------

    /// <summary>
    /// Height of the CharacterController while crouching.
    /// </summary>
    [Header("Collider heights")]
    [SerializeField] private float crouchingHeight = 1.0f;

    // ---------------- Input ----------------

    /// <summary>
    /// Input action for reading WASD movement.
    /// </summary>
    [Header("Input (WASD)")]
    [SerializeField]
    private InputAction moveAction =
        new InputAction(type: InputActionType.Value);

    /// <summary>
    /// Reference to the CharacterController component.
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// Reference to the player GameObject.
    /// Used for existence checks.
    /// </summary>
    [SerializeField] private GameObject player;

    /// <summary>
    /// Vertical velocity component, preserved between frames.
    /// </summary>
    private Vector3 verticalVelocity;

    /// <summary>
    /// Indicates whether the player is currently crouching.
    /// </summary>
    private bool isCrouching = false;

    /// <summary>
    /// Current movement speed (walk / crouch).
    /// </summary>
    private float currentSpeed;

    /// <summary>
    /// Default CharacterController height (standing).
    /// </summary>
    private float defaultHeight;

    /// <summary>
    /// Default CharacterController center (standing).
    /// </summary>
    private Vector3 defaultCenter;

    /// <summary>
    /// Enables the movement input action.
    /// </summary>
    void OnEnable() => moveAction.Enable();

    /// <summary>
    /// Disables the movement input action.
    /// </summary>
    void OnDisable() => moveAction.Disable();

    /// <summary>
    /// Sets up default keyboard bindings for WASD movement
    /// if no bindings are assigned in the editor.
    /// </summary>
    void OnValidate()
    {
        if (moveAction.bindings.Count == 0)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Sprint", "<Keyboard>/shift");
        }
    }

    /// <summary>
    /// Initializes controller references and default movement values.
    /// </summary>
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        defaultHeight = controller.height;
        defaultCenter = controller.center;
    }

    /// <summary>
    /// Updates player movement every frame:
    /// - reads input,
    /// - handles jumping, crouching, sprinting,
    /// - applies gravity,
    /// - moves the CharacterController.
    /// </summary>

    private float minYVelocity = 0f;
    private int minTime = 0;

    void Update()
    {
        if (player.IsDestroyed()) return;

        // ---------- 1) Horizontal movement ----------
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 horizontal = new Vector3(input.x, 0f, input.y);

        float minMag = 1f;

        // Prevent faster diagonal movement
        if (horizontal.sqrMagnitude > minMag)
            horizontal.Normalize();

        // ---------- 2) Read keys ----------
        bool jumpPressed =
            Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        bool crouchPressed =
            Keyboard.current != null &&
            (Keyboard.current.cKey.wasPressedThisFrame ||
             Keyboard.current.leftCtrlKey.wasPressedThisFrame);

        // ---------- 3) Ground / jump / crouch logic ----------
        if (controller.isGrounded)
        {
            if (jumpPressed)
            {
                verticalVelocity.y = jumpVelocity;
            }
            else
            {
                if (verticalVelocity.y < minYVelocity)
                    verticalVelocity.y = groundStickY;
            }

            if (crouchPressed)
            {
                isCrouching = !isCrouching;
                ApplyCrouchSettings();
            }

            if (CanSprint() && Keyboard.current.shiftKey.isPressed)
            {
                Debug.Log("ShiftPressed sprinting!");
                horizontal.z = sprintSpeed;

                countDown -= Time.deltaTime;
                if (countDown <= minTime)
                {
                    countDown = sprintTime;
                    horizontal.z = walkSpeed;
                }
            }
        }
        else
        {
            // Apply gravity while airborne
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        horizontal = transform.TransformDirection(horizontal) * currentSpeed;

        // ---------- 4) Combine & Move ----------
        Vector3 finalVelocity = horizontal;
        finalVelocity.y = verticalVelocity.y;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Applies CharacterController height, center,
    /// and movement speed based on crouch state.
    /// </summary>
    private void ApplyCrouchSettings()
    {
        float reduceCenter = 0.5f;
        float reset = 0f;

        if (isCrouching)
        {
            controller.height = crouchingHeight;

            float deltaH = defaultHeight - crouchingHeight;
            controller.center =
                defaultCenter - new Vector3(reset, deltaH * reduceCenter, reset);

            currentSpeed = crouchSpeed;
        }
        else
        {
            controller.height = defaultHeight;
            controller.center = defaultCenter;
            currentSpeed = walkSpeed;
        }
    }

    /// <summary>
    /// Determines whether the player is allowed to sprint.
    /// </summary>
    /// <returns>True if sprinting is allowed.</returns>
    private bool CanSprint()
    {
        bool notCrouching = !isCrouching;
        return notCrouching;
    }
}
