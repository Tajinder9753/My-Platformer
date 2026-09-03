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
    private bool isGrounded = true;
    private bool hitHead = false;
    private RaycastHit2D groundHit;
    private RaycastHit2D ceilingHit;

    //jumping flags
    private bool isFalling = false;
    public int numJumpsUsed = 0;
    private bool isJumping = false;
    private bool endedJumpEarly = false;
    private bool coyoteUsable = true;

    //jumping timers
    private float jumpBufferCounter;
    private float coyoteTimeCounter;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleJumping();
        HandleGravity();
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
            verticalVelocity = 0f;
        }
        else
        {
            hitHead = false;
        }
    }

    #endregion

    #region movement
    //handles horizontal movement, and deceleration when no input is given
    private void HandleMovement()
    {
        bool isMoving = inputHandler.movement.x != 0; //only checking left/right movement 

        if (isMoving)
        {
            Vector2 targetVelocity = inputHandler.movement * moveStats.moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, moveStats.groundAcceleration * Time.fixedDeltaTime);
        }

        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, moveStats.groundDeceleration * Time.fixedDeltaTime);
        }

        moveVelocity.y = verticalVelocity;
        rb.linearVelocity = moveVelocity;
    }

    #endregion

    #region jumping

    //handles vertical movement (jumping)
    private void HandleJumping()
    {

    }

    private void InitiateJump(int numJumps)
    {

    }

    #endregion

    private void HandleGravity()
    {
        if (isGrounded && moveVelocity.y <= 0f)
        {
            verticalVelocity = moveStats.groundingForce;
        }
        else
        {
            float inAirGravity = moveStats.fallAcceleration;
            if (endedJumpEarly && verticalVelocity > 0f)
            {
                inAirGravity *= moveStats.jumpEndEarlyGravityMultiplier;
            }
            verticalVelocity = Mathf.MoveTowards(verticalVelocity, -moveStats.fallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #region timers
    private void UpdateTimers()
    {

    }
    #endregion

}
