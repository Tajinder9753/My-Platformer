using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //references
    private InputHandler inputHandler;
    private Rigidbody2D rb;
    [SerializeField]private Collider2D feetCol;
    [SerializeField] private Collider2D headCol;
    [SerializeField] private PlayerMovementStats moveStats;

    //current velocity of player
    private Vector2 moveVelocity;
    private float verticalVelocity = 0f;

    //collision flags
    public bool isGrounded = true;
    private bool hitHead = false;
    private RaycastHit2D groundHit;
    private RaycastHit2D ceilingHit;

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
            numJumpsUsed = 0;
            if (!isJumping)
            {
                isFastFalling = false;
                verticalVelocity = 0f;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(headCol.bounds.center.x, headCol.bounds.max.y);
        Vector2 boxCastSize = new Vector2(headCol.bounds.size.x, moveStats.ceilingCheckDistance);
        ceilingHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, 0f, moveStats.groundLayer);

        if (ceilingHit.collider != null)
        {
            hitHead = true;
            isFastFalling = true;
            verticalVelocity = 0f;
        }
        else
        {
            hitHead = false;
        }
    }

    #endregion

    #region movement
    //handles movement, and deceleration when no input is given
    private void HandleMovement()
    {
        bool isMoving = inputHandler.movement.x != 0; //only checking left/right movement 

        if (isMoving)
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
                InitiateJump(1);
                inputHandler.jumpWasPressed = false;
            }

            //double jump
            else if (!isGrounded && numJumpsUsed < moveStats.maxjumps)
            {
                InitiateJump(1);
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
            verticalVelocity += moveStats.gravity * gravityMultiplier * Time.fixedDeltaTime;
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
    private void InitiateJump(int numJumps)
    {
        if (!isJumping)
        {
            isJumping = true;
        }
        numJumpsUsed += numJumps;
        verticalVelocity = moveStats.maxJumpVelocity;
    }
    #endregion

    #region timers

    private void UpdateTimers()
    {
        coyoteTimer -= Time.deltaTime;
    }
    #endregion
}
