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
    public float groundCheckDistance;
    public float ceilingCheckDistance;

}
