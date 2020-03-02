using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController character;
    public Transform mainCamera;

    public float speed = 12f;
    public float drag = 5f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 8f;
    public float jumpHeight = 6f;
    public float groundedOffset = -2f;
    public float sprintSpeedFactor = 1.6f;
    public float sprintJumpVelocityFactor = 90f;
    public float crouchHeightFactor = 0.5f;
    public float crouchJumpVelocityFactor = 1.5f;
    public float crouchSpeedFactor = 0.5f;

    public float fallMultiplier = 1.2f;
    public float lowJumpMultiplier = 1.8f;

    public Transform groundCheck;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    float standingHeight;
    float crouchingHeight;

    /**
     * How far the ground check needs be moved around when crouching
     */
    float groundCheckCrouchOffset;

    internal Vector3 velocity;
    bool hittingCeiling;
    bool holdingShift;
    bool holdingCrouch;
    bool isCrouching;

    public bool IsGrounded { get; private set; }
    public bool HasDoubleJumped { get; private set; }
    public bool HasCrouchJumped { get; private set; }
    public bool HasReachedLowerLevelBoundaries { get; private set; }

    void Start()
    {
        standingHeight = character.height;
        crouchingHeight = character.height * crouchHeightFactor;
        groundCheckCrouchOffset = (standingHeight - crouchingHeight) / 2;
    }

    void ResetForwardVelocity()
    {
        velocity.z = 0f;
        velocity.x = 0f;
    }

    void ResetJumpVelocity()
    {
        velocity.y = groundedOffset;
    }

    void ResetVelocity()
    {
        ResetForwardVelocity();
        ResetJumpVelocity();
    }

    void Land()
    {
        ResetVelocity();
        HasDoubleJumped = false;
        HasCrouchJumped = false;
    }

    void ReachedLowerLevelBoundaries()
    {
        if (HasReachedLowerLevelBoundaries)
        {
            return;
        }

        HasReachedLowerLevelBoundaries = true;
        // Assumption: There is only one game manager and it is available when starting the game from the welcome scene.
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.LevelFailed();
        } else
        {
            Debug.Log("Player has reached lower level boundaries");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Did we fall of the level?
        if (transform.position.y < -2.5f)
        {
            ReachedLowerLevelBoundaries();
        }

        // Is our groundCheck transform intersecting with the ground layer?
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

        // Is the camera intersecting with the ground layer? (Not super accurate, but does the job)
        hittingCeiling = Physics.CheckSphere(mainCamera.position, 1f, groundMask, QueryTriggerInteraction.Ignore);

        holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        holdingCrouch = Input.GetKey(KeyCode.LeftControl);
        bool pressedJump = Input.GetButtonDown("Jump");
        bool holdingJump = Input.GetButton("Jump");

        bool standingBlockedByCeiling = isCrouching && hittingCeiling && !holdingCrouch;

        float actualGravity = gravity * gravityMultiplier;

        // Crouching
        if (holdingCrouch && !isCrouching)
        {
            character.height = crouchingHeight;
            groundCheck.Translate(Vector3.up * groundCheckCrouchOffset);
            isCrouching = true;
        }

        // Stand back up
        if (!holdingCrouch && isCrouching && !hittingCeiling)
        {
            character.height = standingHeight;
            groundCheck.Translate(Vector3.down * groundCheckCrouchOffset);
            isCrouching = false;
        }

        if (velocity.y < 0)
        {
            // Contact with floor
            if (IsGrounded)
            {
                Land();
            } else // Falling down faster
            {
                actualGravity *= fallMultiplier;
            }
        }

        // Apply more gravity when moving upwards and not holding the jump button
        // -> Low jump when only tapping the space bar.
        if (velocity.y > 0 && !holdingJump)
        {
            actualGravity *= lowJumpMultiplier;
        } 

        // Contact with ceiling while jumping -> Apply inverse y velocity
        if (!IsGrounded && (character.collisionFlags & CollisionFlags.Above) != 0 && velocity.y > 0)
        {
            velocity.y = -velocity.y * 0.5f;
        }

        // Unity recognizes "a" as 1 and "d" as -1
        float x = Input.GetAxis("Horizontal");

        // Unity recognizes "w" as 1 and "s" as -1
        float z = Input.GetAxis("Vertical");

        float actualSpeed = speed;

        // Sprint speed
        if (z == 1 && x == 0 && holdingShift && IsGrounded && !isCrouching)
        {
            actualSpeed *= sprintSpeedFactor;
        }

        // Decrease speed when crouching or when in the air after crouch jumping
        if (isCrouching && IsGrounded || HasCrouchJumped)
        {
            actualSpeed *= crouchSpeedFactor;
        }

        // Move player according to x, z input and speed
        Vector3 horizontalMove = transform.right * x;
        Vector3 verticalMove = transform.forward * z;
        Vector3 move = horizontalMove + verticalMove;

        Vector3 motion = move * actualSpeed * Time.deltaTime;
        character.Move(motion);

        // Faster crouch hack
        if (holdingCrouch && IsGrounded && !holdingJump)
        {
            actualGravity *= 10;
        }

        // Jumping
        if (pressedJump && !standingBlockedByCeiling)
        {
            if (IsGrounded)
            {
                // Regular jump: Increase upwards velocity
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);

                // Sprint jump
                if (holdingShift && !isCrouching)
                {
                    // Increase forward velocity
                    velocity.x = motion.x * sprintJumpVelocityFactor;
                    velocity.z = motion.z * sprintJumpVelocityFactor;

                    // Decrease upwards velocity
                    velocity.y *= 0.8f;
                }


                // Crouch jump
                if (holdingCrouch && !hittingCeiling)
                {
                    // Increase upwards velocity
                    velocity.y *= crouchJumpVelocityFactor;

                    // In order to decrease forwards motion
                    HasCrouchJumped = true;
                }
            }

            // Not grounded -> Double jump
            else if (!HasDoubleJumped)
            {
                ResetForwardVelocity();
                HasCrouchJumped = false;
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                HasDoubleJumped = true;
            }
        }

        // Gravity
        velocity.y += actualGravity * Time.deltaTime;

        // Apply velocity
        character.Move(velocity * Time.deltaTime);
    }
}
