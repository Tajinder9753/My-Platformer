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

    [Header("Jump Timers")]
    public float timeTillJumpApex;
    public float jumpHoldTime;
    public float coyoteTime;

    [Header("Gravity Stats")]
    public float gravityScale;

    [Header("Collision Stats")]
    public float groundCheckDistance;
    public float ceilingCheckDistance;

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
        gravityScale = -(2f * jumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravityScale) * timeTillJumpApex;
    }
}
