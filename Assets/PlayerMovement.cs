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

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    bool hasDoubleJumped = false;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Contact with floor.
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = groundedOffset;
            hasDoubleJumped = false;
        }

        // Unity recognizes "w" as 1 and "s" as -1.
        float x = Input.GetAxis("Horizontal");

        // Unity recognizes "a" as 1 and "d" as -1.
        float z = Input.GetAxis("Vertical");

        // Move player according to x, z input and speed.
        Vector3 horizontalMove = transform.right * x;
        Vector3 verticalMove = transform.forward * z;
        Vector3 move = horizontalMove + verticalMove;
        controller.Move(move * speed * Time.deltaTime);

        float actualGravity = gravity * gravityMultiplier;

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
