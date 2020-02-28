using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController character;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 8f;
    public float jumpHeight = 6f;
    public float groundedOffset = -2f;
    public float sprintFactor = 2f;
    public float sprintJumpBoost = 50f;
    public float crouchFactor = 0.5f;
    public float crouchSpeedFactor = 0.1f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    float standingHeight;
    float crouchingHeight;

    /**
     * How far the ground check needs be moved around when crouching
     */
    float groundCheckCrouchOffset;

    internal Vector3 velocity;
    bool isGrounded;
    bool hasDoubleJumped = false;
    bool holdingShift = false;
    bool holdingCrouch = false;
    bool isCrouching = false;

    void Start()
    {
        standingHeight = character.height;
        crouchingHeight = character.height * crouchFactor;
        groundCheckCrouchOffset = (standingHeight - crouchingHeight) / 2;
    }

    void ResetForwardVelocity()
    {
        velocity.z = 0f;
        velocity.x = 0f;
    }

    void ResetVelocity()
    {
        velocity.y = groundedOffset;
        ResetForwardVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        holdingCrouch = Input.GetKey(KeyCode.LeftControl);

        // Crouching
        if (holdingCrouch && !isCrouching)
        {
            character.height = crouchingHeight;
            groundCheck.Translate(Vector3.up * groundCheckCrouchOffset);
            isCrouching = true;
        }

        // Stand back up
        if (!holdingCrouch && isCrouching)
        {
            character.height = standingHeight;
            groundCheck.Translate(Vector3.down * groundCheckCrouchOffset);
            isCrouching = false;
        }

        // Contact with floor
        if (isGrounded && velocity.y < 0)
        {
            ResetVelocity();
            hasDoubleJumped = false;
        }

        // Unity recognizes "a" as 1 and "d" as -1.
        float x = Input.GetAxis("Horizontal");

        // Unity recognizes "w" as 1 and "s" as -1.
        float z = Input.GetAxis("Vertical");

        float actualSpeed = speed;

        // Sprint speed
        if (z == 1 && x == 0 && holdingShift && isGrounded && !isCrouching)
        {
            actualSpeed *= sprintFactor;
        }

        // Crouch speed
        if (isCrouching && isGrounded)
        {
            actualSpeed *= crouchSpeedFactor;
        }

        // Move player according to x, z input and speed.
        Vector3 horizontalMove = transform.right * x;
        Vector3 verticalMove = transform.forward * z;
        Vector3 move = horizontalMove + verticalMove;
        Vector3 motion = move * actualSpeed * Time.deltaTime;
        character.Move(motion);

        float actualGravity = gravity * gravityMultiplier;

        // Faster crouch hack
        if (holdingCrouch && isGrounded)
        {
            actualGravity *= 20;
        }

        // Regular jump
        if (!holdingCrouch && Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // Sprint jump
                if (holdingShift)
                {
                    velocity.x = motion.x * sprintJumpBoost;
                    velocity.z = motion.z * sprintJumpBoost;
                }

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
            }

            // Double jump
            else if (!hasDoubleJumped)
            {
                ResetForwardVelocity();
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                hasDoubleJumped = true;
            }
        }

        velocity.y += actualGravity * Time.deltaTime;

        character.Move(velocity * Time.deltaTime);
    }
}
