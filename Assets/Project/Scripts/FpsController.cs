using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FpsController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxInputAcceleration = 15.0f;
    [SerializeField] private float horizontalDamping = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;

    [Header("Ground Check")]
    [SerializeField] private float groundRayDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float sensitivityX = 1.0f;
    [SerializeField] private float sensitivityY = 1.0f;
    [SerializeField] private float minPitch = -89.0f;
    [SerializeField] private float maxPitch = +89.0f;

    private Vector3 horizontalInputAcceleration = Vector3.zero;
    private Vector3 verticalInputAcceleration = Vector3.zero;
    private Rigidbody rb;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Hide and lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation variables
        yaw = transform.eulerAngles.y;
        if (cameraTransform != null)
        {
            pitch = cameraTransform.localEulerAngles.x;
        }
    }

    void Update()
    {
        this.HandleCameraInput();
        this.ProcessMovementInput();
    }

    void FixedUpdate()
    {
        this.UpdatePhysics();
    }

    void HandleCameraInput()
    {
        if (Mouse.current == null || cameraTransform == null){return;};

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float deltaX = mouseDelta.x * sensitivityX * 0.1f;
        float deltaY = mouseDelta.y * sensitivityY * 0.1f;

        yaw += deltaX;
        pitch -= deltaY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        
        // Apply yaw (left/right) to the main player body
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Apply pitch (up/down) locally to the camera
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void ProcessMovementInput()
    {
        if (Keyboard.current == null) return;

        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;

        Vector3 inputDir = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) inputDir += camForward;
        if (Keyboard.current.sKey.isPressed) inputDir -= camForward;
        if (Keyboard.current.dKey.isPressed) inputDir += camRight;
        if (Keyboard.current.aKey.isPressed) inputDir -= camRight;

        if (inputDir.sqrMagnitude != 0.0f)
        {
            inputDir.Normalize();
            horizontalInputAcceleration = inputDir * maxInputAcceleration;
        }
        else
        {
            horizontalInputAcceleration = Vector3.zero;
        }

        // Jump input
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsOnGround())
        {
            verticalInputAcceleration = Vector3.up * jumpForce;
        }
    }

    bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundRayDistance, groundLayer);
    }

    void UpdatePhysics()
    {
        // Apply horizontal movement
        rb.AddForce(horizontalInputAcceleration * 5.0f, ForceMode.Acceleration);

        // Apply jump impulse and reset vertical input
        if (verticalInputAcceleration != Vector3.zero)
        {
            rb.AddForce(verticalInputAcceleration * 60.0f, ForceMode.Impulse);
            verticalInputAcceleration = Vector3.zero;
        }

        // Apply horizontal drag manually
        Vector3 vel = rb.linearVelocity;
        vel.x *= Mathf.Clamp01(1f - horizontalDamping * Time.fixedDeltaTime);
        vel.z *= Mathf.Clamp01(1f - horizontalDamping * Time.fixedDeltaTime);
        rb.linearVelocity = vel;
    }
}