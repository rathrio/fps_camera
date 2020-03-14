using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 150f;

    public Transform playerBody;

    public PlayerInput playerInput;

    /**
     * Rotation AROUND the x axis.
     */
    float xRotation;

    Vector2 input;

    public void Rotate(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                input = context.ReadValue<Vector2>();
                break;
            case InputActionPhase.Canceled:
                input = Vector2.zero;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide and lock cursor to the center of the screen, so that player can't move mouse cursor out of window.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = input.x * mouseSensitivity * Time.deltaTime;
        float mouseY = input.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        // Limit the rotation of the camera to 180 degrees.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera around the x axis.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate body with respect to x only.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}