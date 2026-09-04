using UnityEngine;

public class InputHandler : MonoBehaviour, PlayerInput.IPlayerActions
{
    private PlayerInput playerInputActions;
    public Vector2 movement;
    public bool jumpWasPressed;
    public bool jumpIsHeld;

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
        if (context.started)
        {
            jumpWasPressed = true;
            jumpIsHeld = true;
        }
        else if (context.canceled)
        {
            jumpWasPressed = false;
            jumpIsHeld = false;
        }
    }

    public void OnMovement(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
}
