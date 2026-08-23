using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CustomCharachterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxInputAcceleration = 15.0f;
    [SerializeField] private float horizontalDamping = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    [Header("Ground Check")]
    [SerializeField] private float groundRayDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 horizontalInputAcceleration = Vector3.zero;
    private Vector3 verticalInputAcceleration = Vector3.zero;
    private Quaternion cameraDirection = Quaternion.identity;

    private Rigidbody rb = null;

    void ProcessInput()
    {
        if (Keyboard.current == null) return;

        Vector3 camEuler = cameraDirection.eulerAngles;
        Quaternion cameraYaw = Quaternion.Euler(0f, camEuler.y, 0f);

        Vector3 camForward = cameraYaw * Vector3.forward;
        Vector3 camRight = cameraYaw * Vector3.right;

        Vector3 inputDir = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) inputDir += camForward;
        if (Keyboard.current.sKey.isPressed) inputDir -= camForward;
        if (Keyboard.current.dKey.isPressed) inputDir += camRight;
        if (Keyboard.current.aKey.isPressed) inputDir -= camRight;

        if (inputDir.sqrMagnitude != 0.0f)
        {
            inputDir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            horizontalInputAcceleration = inputDir.normalized * maxInputAcceleration;
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

    public void SetCameraDirection(Quaternion direction)
    {
        this.cameraDirection = direction;
    }

    // Unity fucntions
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        UpdatePhysics();
    }
}