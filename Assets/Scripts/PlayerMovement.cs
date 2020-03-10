using UnityEngine;

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

    float _standingHeight;
    float _crouchingHeight;

    /**
     * How far the ground check needs be moved around when crouching
     */
    float _groundCheckCrouchOffset;

    private Vector3 _upwardsVelocity;
    private Vector3 _forwardVelocity = new Vector3(0f, 0f, 0f);
    
    bool _holdingShift;
    bool _holdingCrouch;
    bool _isCrouching;

    public bool IsGrounded { get; private set; }
    public bool HittingCeiling { get; private set; }
    public bool HasDoubleJumped { get; private set; }
    bool HasJumped { get; set; }
    bool HasCrouchJumped { get; set; }
    bool HasSprintJumped { get; set; }
    bool HasReachedLowerLevelBoundaries { get; set; }

    void Start()
    {
        float height = character.height;

        _standingHeight = height;
        _crouchingHeight = height * crouchHeightFactor;
        _groundCheckCrouchOffset = (_standingHeight - _crouchingHeight) / 2;
    }

    void ResetVelocity()
    {
        _upwardsVelocity.y = groundedOffset;
        
        _forwardVelocity.x = 0f;
        _forwardVelocity.z = 0f;
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

        _holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        _holdingCrouch = Input.GetKey(KeyCode.LeftControl);
        bool pressedJump = Input.GetButtonDown("Jump");
        bool holdingJump = Input.GetButton("Jump");

        bool standingBlockedByCeiling = _isCrouching && HittingCeiling && !_holdingCrouch;

        float actualGravity = gravity * gravityMultiplier;

        // Crouching
        if (_holdingCrouch && !_isCrouching)
        {
            character.height = _crouchingHeight;
            groundCheck.Translate(Vector3.up * _groundCheckCrouchOffset);
            _isCrouching = true;
        }

        // Stand back up
        if (!_holdingCrouch && _isCrouching && !HittingCeiling)
        {
            character.height = _standingHeight;
            groundCheck.Translate(Vector3.down * _groundCheckCrouchOffset);
            _isCrouching = false;
        }

        if (_upwardsVelocity.y < groundedOffset)
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
        if (_upwardsVelocity.y > 0 && !holdingJump)
        {
            actualGravity *= lowJumpMultiplier;
        }

        // Contact with ceiling while jumping -> Apply inverse y velocity
        if (!IsGrounded && (character.collisionFlags & CollisionFlags.Above) != 0 && _upwardsVelocity.y > 0)
        {
            _upwardsVelocity.y = 0f;
        }

        // Unity recognizes "a" as 1 and "d" as -1
        float x = Input.GetAxis("Horizontal");

        // Unity recognizes "w" as 1 and "s" as -1
        float z = Input.GetAxis("Vertical");

        bool movingForward = z == 1 && x == 0;
        
        // No horizontal or vertical input
        bool hasMovementInput = z != 0 || x != 0;

        float actualSpeed = speed;

        // Sprint speed
        if (movingForward && (HasSprintJumped || (_holdingShift && IsGrounded && !_isCrouching)))
        {
            actualSpeed *= sprintSpeedFactor;
        }

        // Decrease speed when crouching or when in the air after crouch jumping
        if (_isCrouching && IsGrounded || HasCrouchJumped)
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
            character.Move(motion * 0.2f);
        }

        // Faster crouch hack
        if (_holdingCrouch && IsGrounded && !holdingJump)
        {
            actualGravity *= 10;
        }

        // Jumping
        if (pressedJump && !standingBlockedByCeiling)
        {
            if (IsGrounded)
            {
                // Regular jump: Increase upwards velocity
                _upwardsVelocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                if (hasMovementInput)
                {
                    _forwardVelocity.x = move.x * actualSpeed * 0.9f;
                    _forwardVelocity.z = move.z * actualSpeed * 0.9f;
                }
                
                HasJumped = true;
                
                // Sprint jump
                if (_holdingShift && !_isCrouching)
                {
                    _upwardsVelocity.y *= 0.85f;
                    HasSprintJumped = true;
                }

                // Crouch jump
                if (_holdingCrouch && !HittingCeiling)
                {
                    _upwardsVelocity.y *= crouchJumpVelocityFactor;
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

                _upwardsVelocity.y = Mathf.Sqrt(jumpHeight * -2f * actualGravity);
                if (hasMovementInput)
                {
                    _forwardVelocity.x = move.x * actualSpeed * 0.7f;
                    _forwardVelocity.z = move.z * actualSpeed * 0.7f;
                }
                
                HasDoubleJumped = true;
            }
        }

        // Gravity
        _upwardsVelocity.y += actualGravity * Time.deltaTime;

        Vector3 velocity = _upwardsVelocity + _forwardVelocity;
        
        // Apply velocity
        character.Move(velocity * Time.deltaTime);
    }
}