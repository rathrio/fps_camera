using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController character;
    public Transform mainCamera;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 8f;
    public float jumpHeight = 6f;
    public float groundedOffset = -2f;
    public float sprintSpeedFactor = 1.6f;
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

    private Vector3 upwardsVelocity;
    private Vector3 forwardVelocity = new Vector3(0f, 0f, 0f);

    // Left/right input
    float x;
    // Forwards/backwards input
    float z;
    
    bool holdingShift;
    bool holdingCrouch;
    bool isCrouching;

    public bool IsGrounded { get; private set; }
    public bool HittingCeiling { get; private set; }
    public bool HasDoubleJumped { get; private set; }
    bool HasJumped { get; set; }
    bool HasCrouchJumped { get; set; }
    bool HasSprintJumped { get; set; }
    bool HasReachedLowerLevelBoundaries { get; set; }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 v = context.ReadValue<Vector2>();
        
        x = v.x;
        z = v.y;
    }
    
    public void Crouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                holdingCrouch = true;
                break;
            case InputActionPhase.Canceled:
                holdingCrouch = false;
                break;
        }
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }
    
    public void Sprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                holdingShift = true;
                break;
            case InputActionPhase.Canceled:
                holdingShift = false;
                break;
        }
    }
    
    void Start()
    {
        float height = character.height;

        standingHeight = height;
        crouchingHeight = height * crouchHeightFactor;
        groundCheckCrouchOffset = (standingHeight - crouchingHeight) / 2;
    }

    void ResetVelocity()
    {
        upwardsVelocity.y = groundedOffset;
        
        forwardVelocity.x = 0f;
        forwardVelocity.z = 0f;
    }

    void Land()
    {
        ResetVelocity();
        HasJumped = false;
        HasDoubleJumped = false;
        HasCrouchJumped = false;
        HasSprintJumped = false;
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
        }
        else
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
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask,
            QueryTriggerInteraction.Ignore);

        // Is the camera intersecting with the ground layer? (Not super accurate, but does the job for the demo)
        HittingCeiling = Physics.CheckSphere(mainCamera.position, 1f, groundMask, QueryTriggerInteraction.Ignore);

        bool pressedJump = Input.GetButtonDown("Jump");
        bool holdingJump = Input.GetButton("Jump");

        bool standingBlockedByCeiling = isCrouching && HittingCeiling && !holdingCrouch;

        float actualGravity = gravity * gravityMultiplier;

        // Crouching
        if (holdingCrouch && !isCrouching)
        {
            character.height = crouchingHeight;
            groundCheck.Translate(Vector3.up * groundCheckCrouchOffset);
            isCrouching = true;
        }

        // Stand back up
        if (!holdingCrouch && isCrouching && !HittingCeiling)
        {
            character.height = standingHeight;
            groundCheck.Translate(Vector3.down * groundCheckCrouchOffset);
            isCrouching = false;
        }

        if (upwardsVelocity.y < groundedOffset)
        {
            // Contact with floor. TODO: Find a way to not call this non-stop on the ground.
            if (IsGrounded)
            {
                Land();
            }
            else // Falling down faster
            {
                actualGravity *= fallMultiplier;
            }
        }

        // Apply more gravity when moving upwards and not holding the jump button
        // -> Low jump when only tapping the space bar.
        if (upwardsVelocity.y > 0 && !holdingJump)
        {
            actualGravity *= lowJumpMultiplier;
        }

        // Contact with ceiling while jumping -> Apply inverse y velocity
        if (!IsGrounded && (character.collisionFlags & CollisionFlags.Above) != 0 && upwardsVelocity.y > 0)
        {
            upwardsVelocity.y = 0f;
        }

        // // A/D
        // float x = Input.GetAxis("Horizontal");
        //
        // // W/S
        // float z = Input.GetAxis("Vertical");

        bool movingForward = z == 1 && Math.Abs(x) < 0.3f;
        
        // No horizontal or vertical input
        bool hasMovementInput = z != 0 || x != 0;

        float actualSpeed = speed;

        // Sprint speed
        if (movingForward && (HasSprintJumped || (holdingShift && IsGrounded && !isCrouching)))
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
        Vector3 motion = move * (actualSpeed * Time.deltaTime);
        
        if (IsGrounded)
        {
            character.Move(motion);
        }
        else
        {
            character.Move(motion * 0.3f);
        }

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
                upwardsVelocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                if (hasMovementInput)
                {
                    forwardVelocity.x = move.x * actualSpeed * 0.8f;
                    forwardVelocity.z = move.z * actualSpeed * 0.8f;
                }
                
                HasJumped = true;
                
                // Sprint jump
                if (holdingShift && !isCrouching)
                {
                    upwardsVelocity.y *= 0.85f;
                    HasSprintJumped = true;
                }

                // Crouch jump
                if (holdingCrouch && !HittingCeiling)
                {
                    upwardsVelocity.y *= crouchJumpVelocityFactor;
                    // In order to decrease forwards motion
                    HasCrouchJumped = true;
                }
            }

            // Not grounded -> Double jump
            else if (!HasDoubleJumped)
            {
                HasJumped = false;
                HasCrouchJumped = false;
                HasSprintJumped = false;

                upwardsVelocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                if (hasMovementInput)
                {
                    forwardVelocity.x = move.x * actualSpeed * 0.7f;
                    forwardVelocity.z = move.z * actualSpeed * 0.7f;
                }
                
                HasDoubleJumped = true;
            }
        }

        // Gravity
        upwardsVelocity.y += actualGravity * Time.deltaTime;

        Vector3 velocity = upwardsVelocity + forwardVelocity;
        
        // Apply velocity
        character.Move(velocity * Time.deltaTime);
    }
}