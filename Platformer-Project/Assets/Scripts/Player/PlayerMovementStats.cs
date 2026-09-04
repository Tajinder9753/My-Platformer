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

    [Header("Collision Stats")]
    //raycast distance for ground and ceiling checks
    public float groundCheckDistance;
    public float ceilingCheckDistance;

    [Header("Edge Assist Stats")]
    public float cornerCheckDistance;
    public float nudgeDistance;

    [Header("Jump Stats")]
    public float maxJumpHeight; //how high will jump
    public float minJumpHeight; //shorter jump height for buffered jump
    public float timeTillJumpApex; //time it takes to reach the apex of the jump
    public float gravityOnReleaseMultiplier; //multiplier for gravity when jump button is released early
    public float maxFallSpeed; //maximum speed the player can fall
    public int maxjumps;

    [Header("Jump Buffer")]
    public float jumpBufferTime; //how long the player can buffer a jump input before landing

    [Header("Jump Coyote Time")]
    public float jumpCoyoteTime; //how long the player can jump after leaving the ground

    public float gravity;
    public float maxJumpVelocity;
    public float minJumpVelocity; //for buffered jump 

    [Header("Wall Slide Values")]
    public float wallSlideGravityMultiplier;

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        gravity = -(2f * maxJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        maxJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }
}
