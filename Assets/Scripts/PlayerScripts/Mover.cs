using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleKeyboardMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpVelocity = 6f;
    [SerializeField] private float groundStickY = -1f;   // small push down when grounded

    [Header("Collider heights")]
    [SerializeField] private float crouchingHeight = 1.0f; // standing height is read from controller

    [Header("Input (WASD)")]
    [SerializeField]
    private InputAction moveAction =
        new InputAction(type: InputActionType.Value);

    private CharacterController controller;

    // verticalVelocity.y is kept between frames
    private Vector3 verticalVelocity;

    private bool isCrouching = false;
    private float currentSpeed;

    // for restoring collider when standing
    private float defaultHeight;
    private Vector3 defaultCenter;

    void OnEnable() => moveAction.Enable();
    void OnDisable() => moveAction.Disable();

    void OnValidate()
    {
        if (moveAction.bindings.Count == 0)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        defaultHeight = controller.height;
        defaultCenter = controller.center;
    }

    void Update()
    {
        // ---------- 1) Horizontal movement ----------
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 horizontal = new Vector3(input.x, 0f, input.y);

        float minMag = 1f;

        if (horizontal.sqrMagnitude > minMag)       // prevent super-fast diagonals
            horizontal.Normalize();

        horizontal = transform.TransformDirection(horizontal) * currentSpeed;

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
                // jump even when standing still
                verticalVelocity.y = jumpVelocity;
            }
            else
            {
                // keep character stuck to ground when not jumping
                if (verticalVelocity.y < 0f)
                    verticalVelocity.y = groundStickY;
            }

            if (crouchPressed)
            {
                isCrouching = !isCrouching;
                ApplyCrouchSettings();
            }
        }
        else
        {
            // apply gravity while in the air
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        // ---------- 4) Combine & Move ----------
        Vector3 finalVelocity = horizontal;
        finalVelocity.y = verticalVelocity.y;

        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void ApplyCrouchSettings()
    {
        float reduceCenter = 0.5f;
        float reset = 0f;
        if (isCrouching)
        {
            controller.height = crouchingHeight;

            // lower center so feet stay on the ground
            float deltaH = defaultHeight - crouchingHeight;
            controller.center = defaultCenter - new Vector3(reset, deltaH * reduceCenter, reset);

            currentSpeed = crouchSpeed;
        }
        else
        {
            controller.height = defaultHeight;
            controller.center = defaultCenter;
            currentSpeed = walkSpeed;
        }
    }
}
