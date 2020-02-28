using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 8f;
    public float jumpHeight = 6f;
    public float groundedOffset = -1f;
    public float sprintFactor = 2f;
    public float crouchFactor = 0.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool hasDoubleJumped = false;
    bool holdingShift = false;
    bool holdingCrouch = false;
    bool isCrouching = false;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        holdingCrouch = Input.GetKey(KeyCode.LeftControl);

        // Crouching
        if (holdingCrouch && !isCrouching)
        {
            controller.height *= crouchFactor;
            isCrouching = true;
        }

        // Stand back up
        if (!holdingCrouch && isCrouching)
        {
            isCrouching = false;
            controller.height /= crouchFactor;
        }

        // Contact with floor
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = groundedOffset;
            hasDoubleJumped = false;
        }

        // Unity recognizes "a" as 1 and "d" as -1.
        float x = Input.GetAxis("Horizontal");

        // Unity recognizes "w" as 1 and "s" as -1.
        float z = Input.GetAxis("Vertical");

        float actualSpeed = speed;
        if (z == 1 && x == 0 && holdingShift && isGrounded)
        {
            actualSpeed *= sprintFactor;
        }

        // Move player according to x, z input and speed.
        Vector3 horizontalMove = transform.right * x;
        Vector3 verticalMove = transform.forward * z;
        Vector3 move = horizontalMove + verticalMove;

        controller.Move(move * actualSpeed * Time.deltaTime);

        float actualGravity = gravity * gravityMultiplier;
        if (holdingCrouch)
        {
            actualGravity *= 10;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
            } else if (!hasDoubleJumped)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                hasDoubleJumped = true;
            }
        }

        velocity.y += actualGravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
