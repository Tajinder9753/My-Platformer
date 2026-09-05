using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //references
    private InputHandler inputHandler;
    private Rigidbody2D rb;
    [SerializeField] private Collider2D feetCol;
    [SerializeField] private Collider2D bodyCol;
    [SerializeField] private PlayerMovementStats moveStats;

    //current velocity of player
    private Vector2 moveVelocity;
    private float verticalVelocity = 0f;

    //collision flags
    public bool isGrounded = true;
    private bool hitHead = false;
    private RaycastHit2D groundHit;
    public bool isWallSliding = false;
    private bool isTouchingWall = false;
    private RaycastHit2D wallHit;
    private RaycastHit2D lastWallHit;

    //flip check flag 
    private bool isFacingRight = true;

    //jumping flags
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isFastFalling = true;
    private bool jumpReleasedEarly = false;
    private bool wasGrounded = true;

    //jumping variables 
    private float jumpBufferTimer = 0f;
    private int numJumpsUsed = 0;
    private float coyoteTimer = 0f;
    private float jumpHoldTimer = 0f;
    private float gravityMultiplier;
    private float wallJumpCoyoteTimer = 0f;


    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleJumping();
        HandleMovement();
    }

    #region Collision Checks

    //checks for collisions with the ground and ceiling 
    private void CheckCollisions()
    {
        IsGrounded();
        BumpedHead();
        TouchingWall();
    }

    //checks if the player is grounded by performing a boxcast downwards from the feet collider 
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCol.bounds.center.x, feetCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCol.bounds.size.x, moveStats.groundCheckDistance);
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0f, moveStats.groundLayer);

        if (groundHit.collider != null)
        {
            isGrounded = true;
            if (!isJumping)
            {
                isFastFalling = false;
                isWallSliding = false;
                numJumpsUsed = 0;
                verticalVelocity = 0f;
                wallJumpCoyoteTimer = 0f;
            }

            if (jumpBufferTimer > 0f)
            {
                InitiateJump(1, true);
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void BumpedHead()
    {
        float rayLength = moveStats.ceilingCheckDistance;
        float cornerCheckDistance = moveStats.cornerCheckDistance;

        Vector2 leftOrigin = new Vector2(bodyCol.bounds.min.x + cornerCheckDistance, bodyCol.bounds.max.y);
        Vector2 rightOrigin = new Vector2(bodyCol.bounds.max.x - cornerCheckDistance, bodyCol.bounds.max.y);

        RaycastHit2D leftHit = Physics2D.Raycast(leftOrigin, Vector2.up, rayLength, moveStats.groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightOrigin, Vector2.up, rayLength, moveStats.groundLayer);

        bool hitLeft = leftHit.collider != null;
        bool hitRight = rightHit.collider != null;

        //hit a ceiling 
        if (hitLeft && hitRight)
        {
            hitHead = true;
            isFastFalling = true;
            verticalVelocity = 0f;
        }
        //hit an edge, help player move upwards still just away from the ledge 
        else if (hitLeft || hitRight)
        {
            hitHead = false;
            float nudgeDirection = hitLeft ? 1f : -1f;
            transform.position += new Vector3(nudgeDirection * moveStats.nudgeDistance, 0f, 0f);
        }

        else
        {
            hitHead = false;
        }
    }

    private void TouchingWall()
    {
        float originEndPoint = 0f;
        originEndPoint = isFacingRight ? bodyCol.bounds.max.x : bodyCol.bounds.min.x;

        float adjustedHeight = bodyCol.bounds.size.y * moveStats.wallDetectionRayHeightMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, bodyCol.bounds.center.y);
        Vector2 boxCastSize = new Vector2(moveStats.wallDetectionRayLength, adjustedHeight);

        wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, moveStats.wallDetectionRayLength, moveStats.groundLayer);

        bool wasSlidingLastFrame = isWallSliding;

        if (wallHit.collider != null)
        {
            isTouchingWall = true;
            lastWallHit = wallHit;

            if (!isGrounded && verticalVelocity <= 0f)
            {
                isWallSliding = true;
            }
        }
        else
        {
            isTouchingWall = false;
            isWallSliding = false;
        }

        if (wasSlidingLastFrame && !isWallSliding && !isGrounded)
        {
            wallJumpCoyoteTimer = moveStats.wallJumpCoyoteTime;
        }
    }

    #endregion

    #region movement
    //handles movement, and deceleration when no input is given
    private void HandleMovement()
    {
        bool isMoving = inputHandler.movement.x != 0; //only checking left/right movement
        bool movingRight = inputHandler.movement.x > 0;
        bool stopMovement = false;
        if (isWallSliding)
        {
            if (isFacingRight && movingRight)
            {
                moveVelocity.x = 0;
                stopMovement = true;
            }
            else if (!isFacingRight && !movingRight)
            {
                moveVelocity.x = 0;
                stopMovement = true;
            }
            else
            {
                stopMovement = false;
            }
        }

        if (isMoving && !stopMovement)
        {
            FlipCheck(); //checks if need to flip the sprite
            Vector2 targetVelocity = inputHandler.movement * moveStats.moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, moveStats.groundAcceleration * Time.fixedDeltaTime);
        }

        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, moveStats.groundDeceleration * Time.fixedDeltaTime);
        }

        //adds vertical velocity as well from the jump/fall calculations
        moveVelocity.y = verticalVelocity;
        rb.linearVelocity = moveVelocity;
    }

    private void FlipCheck()
    {
        if (isFacingRight && moveVelocity.x < 0)
        {
            Flip(false);
        }
        else if (!isFacingRight && moveVelocity.x > 0)
        {
            Flip(true);
        }
    }

    private void Flip (bool turnRight)
    {
        if (turnRight)
        {
            transform.Rotate(0, 180, 0);
            isFacingRight = true;
        }
        else
        {
            transform.Rotate(0, -180, 0);
            isFacingRight = false;
        }
    }
    #endregion

    #region jumping
    private void JumpChecks()
    {
        //starts coyote timer when not on ground
        if (wasGrounded && !isGrounded)
        {
            coyoteTimer = moveStats.jumpCoyoteTime;
        }

        if (inputHandler.jumpWasPressed)
        {
            //initiate jump if grounded or within coyote time
            if ((isGrounded || coyoteTimer > 0f) && numJumpsUsed < moveStats.maxjumps)
            {
                InitiateJump(1, false);
                inputHandler.jumpWasPressed = false;
            }

            //wall jump
            else if (!isGrounded && (isWallSliding || wallJumpCoyoteTimer > 0f))
            {
                InitiateWallJump();
            }

            //double jump
            else if (!isGrounded && numJumpsUsed < moveStats.maxjumps)
            {
                InitiateJump(1, false);
                inputHandler.jumpWasPressed = false;
            }

            else
            {
                jumpBufferTimer = moveStats.jumpBufferTime;
                inputHandler.jumpWasPressed = false;
            }
        }

        //apply stronger gravity if jump was released early while still ascending
        jumpReleasedEarly = isJumping && !inputHandler.jumpIsHeld && verticalVelocity > 0f;

        wasGrounded = isGrounded;
    }

    private void HandleJumping()
    {

        if (!isGrounded)
        {
            //fall faster if fast falling or released jump button early
            if (isFastFalling || jumpReleasedEarly)
            {
                gravityMultiplier = moveStats.gravityOnReleaseMultiplier;
            }
            else
            {
                gravityMultiplier = 1f;
            }

            bool inCoyoteBuffer = wallJumpCoyoteTimer > 0f;

            if (isWallSliding || inCoyoteBuffer)
            {
                gravityMultiplier = moveStats.wallSlideGravityMultiplier;
                verticalVelocity = moveStats.gravity * gravityMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                verticalVelocity += moveStats.gravity * gravityMultiplier * Time.fixedDeltaTime;

            }
        }

        //resets isJumping when landed
        if (isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
        }

        //clamp fall speed 
        verticalVelocity = Mathf.Clamp(verticalVelocity, -moveStats.maxFallSpeed, moveStats.maxFallSpeed);
    }

    //applying the actual jump velocity 
    private void InitiateJump(int numJumps, bool isBufferedJump)
    {
        if (!isJumping)
        {
            isJumping = true;
        }
        numJumpsUsed += numJumps;
        verticalVelocity = isBufferedJump ? moveStats.minJumpVelocity : moveStats.maxJumpVelocity;
        jumpBufferTimer = 0f;
    }

    private void InitiateWallJump()
    {
        isJumping = true;
        isWallSliding = false;
        isTouchingWall = false;
        wallJumpCoyoteTimer = 0f;

        verticalVelocity = moveStats.wallJumpVelocity;

        int dirMultiplier = 0;
        Vector2 hitPoint = lastWallHit.collider.ClosestPoint(bodyCol.bounds.center);

        //send backwards if facing wall 
        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        //otherwise forwards
        else
        {
            dirMultiplier = 1;
        }

        moveVelocity.x = Mathf.Abs(moveStats.wallJumpDirection.x) * dirMultiplier;
    }

    #endregion

    #region timers

    private void UpdateTimers()
    {
        coyoteTimer -= Time.deltaTime;

        if (!isGrounded)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (!isTouchingWall)
        {
            wallJumpCoyoteTimer -= Time.deltaTime; 
        }
    }
    #endregion
}
