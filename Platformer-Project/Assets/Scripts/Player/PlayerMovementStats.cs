using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementStats", menuName = "Scriptable Objects/PlayerMovementStats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Header("Movement Stats")]
    public float moveSpeed;
    public float groundAcceleration;
    public float groundDeceleration;

    [Header("Jump Stats")]
    public float jumpHeight;
    public float initialJumpVelocity;
    public int maxJumps;
    public float airAcceleration;
    public float airDeceleration;

    [Header("Jump Timers")]
    public float timeTillJumpApex;
    public float jumpBuffer; // allows a jump before hitting the ground 
    public float coyoteTime;

    [Header("Gravity Stats")]
    public float gravityScale;
    public float groundingForce; //constant downward force
    public float jumpEndEarlyGravityMultiplier;

    [Header("Collision Stats")]
    public float groundCheckDistance;
    public float ceilingCheckDistance;

    [Header("Fall Stats")]
    public float fallSpeed;
    public float fallAcceleration;
}
