using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public event System.Action JumpEvent;
    public event System.Action DodgeEvent;
    public event System.Action AttackEvent;
    public event System.Action TargetEvent;
    public event System.Action CancelTargetEvent;

    private Controls controls;

    public Vector2 MovementValue { get; private set; }

    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        JumpEvent?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DodgeEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AttackEvent?.Invoke();
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TargetEvent?.Invoke();
    }

    public void OnCancelTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        CancelTargetEvent?.Invoke();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        // cinemachine ja consome o look direto
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }
}
