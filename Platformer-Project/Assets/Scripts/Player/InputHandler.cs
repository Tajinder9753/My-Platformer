using UnityEngine;

public class InputHandler : MonoBehaviour, PlayerInput.IPlayerActions
{
    private PlayerInput playerInputActions;
    public Vector2 movement;
    public bool jumpWasPressed;
    public bool jumpWasReleased;

    private void Awake()
    {
        playerInputActions = new PlayerInput();
        playerInputActions.Player.SetCallbacks(this);
    }
    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
    public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpWasPressed = true;
            jumpWasReleased = false;
        }
        else if (context.canceled)
        {
            jumpWasPressed = false;
            jumpWasReleased = true;
        }
    }

    public void OnMovement(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
}
