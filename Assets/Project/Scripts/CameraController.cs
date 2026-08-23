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

    void Start()
    {
        Vector3 angles = this.transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void handleInput()
    {
        if (Mouse.current == null) return;

        if ( Mouse.current.rightButton.isPressed)
        {
            // Hide mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Procces input
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            float deltaX = (mouseDelta.x / Screen.width) * sensitivityX * 100.0f;
            float deltaY = (mouseDelta.y / Screen.height) * sensitivityY * 100.0f;

            yaw += deltaX;
            pitch -= deltaY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            this.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            // Shopw mosue
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    void updateCameraPosition()
    {
        if (this.playerTransform == null) return;
        this.transform.position = this.playerTransform.position - (this.transform.forward * distance); // Position camera relative to target rotation and distance
    }

    void updatecontroller()
    {
        CustomCharachterController playerCharacterController = this.playerTransform.GetComponent<CustomCharachterController>();
        if ( playerCharacterController == null ){ return; }

        playerCharacterController.setCameraDirection(this.transform.rotation);
    }

    void LateUpdate()
    {
        this.handleInput();
        this.updateCameraPosition();
        this.updatecontroller();
    }
}