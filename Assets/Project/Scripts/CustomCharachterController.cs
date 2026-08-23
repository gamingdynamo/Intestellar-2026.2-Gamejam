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

    private Vector3 direction = Vector3.forward;
    private Vector3 horizontalInputAcceleration = Vector3.zero;
    private Vector3 verticalInputAcceleration = Vector3.zero;
    private Rigidbody rb = null;
    private Quaternion cameraDirection;

    // Later for animtions
    private bool movedLastFrame = false;
    private bool jumpedLastFrame = false;

    void addHorizontalInput()
    {
        if ( Keyboard.current.wKey.isPressed )
        {
            this.horizontalInputAcceleration += this.direction;
        }

        if ( Keyboard.current.sKey.isPressed)
        {
            this.horizontalInputAcceleration += (-this.direction);
        }

        if ( Keyboard.current.dKey.isPressed)
        {
            this.horizontalInputAcceleration += (-1) * (Vector3.Cross(this.direction, Vector3.up));
        }

        if ( Keyboard.current.aKey.isPressed)
        {
            this.horizontalInputAcceleration += Vector3.Cross(this.direction, Vector3.up);
        }

        if (this.horizontalInputAcceleration.sqrMagnitude >= 0.1f)
        {
            this.movedLastFrame = true;

            this.direction = this.cameraDirection * Vector3.forward;

            // Calculate target rotation while neutralizing pitch (X) and roll (Z)
            Vector3 targetEuler = this.cameraDirection.eulerAngles;
            Quaternion targetRotation = Quaternion.Euler(0f, targetEuler.y, 0f);

            // Smoothly interpolate towards the target rotation
            this.transform.rotation = Quaternion.Slerp(
                this.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            this.movedLastFrame = false;
        }

        // Normalize inputAcceleratin and make flat
        this.horizontalInputAcceleration.y = 0.0f;
        this.horizontalInputAcceleration = this.horizontalInputAcceleration.normalized * this.maxInputAcceleration;
    }

    void addVerticalInput()
    {
        // Vertical input
        if ( Keyboard.current.spaceKey.wasPressedThisFrame && this.isOnground() )
        {
            this.verticalInputAcceleration += Vector3.up * jumpForce;
            this.jumpedLastFrame = true;
        }
        else
        {
            this.jumpedLastFrame = false;
        }
    }

    void proccesInput()
    {

        // Get layout indepndent key input
        if (Keyboard.current == null) return;

        // Reset imput vectors
        this.horizontalInputAcceleration = Vector3.zero;
        this.verticalInputAcceleration = Vector3.zero;

        // Now add input vectors
        this.addHorizontalInput();
        this.addVerticalInput();
    }

    bool isOnground()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundRayDistance, groundLayer);
    }

    private void ApplyCustomDamping()
    {
        // Custom damping applied strictly to X and Z, leaving Y (gravity/jumping) completely untouched
        Vector3 vel = rb.linearVelocity;
        vel.x *= Mathf.Clamp01(1f - horizontalDamping * Time.fixedDeltaTime);
        vel.z *= Mathf.Clamp01(1f - horizontalDamping * Time.fixedDeltaTime);

        rb.linearVelocity = vel;
    }

    void updatePhysics()
    {

        // Apply input acceleration
        rb.AddForce( this.horizontalInputAcceleration * 5.0f, ForceMode.Acceleration );
        rb.AddForce( this.verticalInputAcceleration * 60.0f, ForceMode.Impulse );

        // apply 'drag'
        rb.linearDamping = 0.0f; // Disable unitys system, we use our own
        this.ApplyCustomDamping();
    }

    // Unity physics
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        this.proccesInput();
    }

    void LateUpdate()
    {
        this.updatePhysics();
    }

    public void setCameraDirection(Quaternion direction)
    {
        this.cameraDirection = direction;
    }
}
