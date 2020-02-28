using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    /**
     * Rotation AROUND the x axis.
     */
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Hide and lock cursor to the center of the screen, so that player can't move mouse cursor out of window.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        // Limit the rotation of the camera to 180 degrees.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera around the x axis.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate body with respect to x only.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}