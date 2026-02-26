using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLookAndMove : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    public float moveSpeed = 5f;   // Movement speed
    public float gravity = -9.81f; // Gravity for realistic movement

    float xRotation = 0f;
    CharacterController controller;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // --- LOOK AROUND ---
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        // --- PLAYER MOVEMENT ---
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- GRAVITY ---
        if (!controller.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = -0.5f; // small downward force to stick to ground

        controller.Move(velocity * Time.deltaTime);
    }
}