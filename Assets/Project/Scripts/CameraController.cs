using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float sensitivityX = 1.0f;
    [SerializeField] private float sensitivityY = 1.0f;
    [SerializeField] private float minPitch = -80.0f;
    [SerializeField] private float maxPitch = 80.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    void UpdatePlayerController()
    {
        if (playerTransform == null){ return; }

        CustomCharachterController playerController = playerTransform.GetComponent<CustomCharachterController>();
        if (playerController == null){return; }

        playerController.SetCameraDirection(transform.rotation);
    }

    void HandleInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            float deltaX = mouseDelta.x * sensitivityX * 0.1f;
            float deltaY = mouseDelta.y * sensitivityY * 0.1f;

            yaw += deltaX;
            pitch -= deltaY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    void SetCameraPosition()
    {
        // Position camera in LateUpdate after player has moved in Update/FixedUpdate
        if (playerTransform == null) return;
        transform.position = playerTransform.position - (transform.forward * distance);
    }

    // unity functions
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void Update()
    {
        this.HandleInput();
        this.UpdatePlayerController();
    }

    void LateUpdate()
    {
        this.SetCameraPosition();
    }
}