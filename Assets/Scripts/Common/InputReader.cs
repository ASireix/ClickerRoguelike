using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/InputReader")]
public class InputReader : ScriptableObject, IAA_Controls.IUIActions, IAA_Controls.IGameplayActions
{
    public event UnityAction<Vector2> moveEvent;
    public event UnityAction clickStartedEvent;
    public event UnityAction clickEndEvent;
    public event UnityAction focusEventStarted;
    public event UnityAction focusEventCanceled;
    public event UnityAction scanEventStarted;
    public event UnityAction scanEventCanceled;

    IAA_Controls controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new IAA_Controls();
            controls.Gameplay.SetCallbacks(this);
            controls.UI.SetCallbacks(this);
        }

        EnablePlayerInput();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (moveEvent != null && context.performed)
        {
            moveEvent.Invoke(context.ReadValue<Vector2>());
        }
        else if (moveEvent != null && context.canceled)
        {
            moveEvent.Invoke(Vector2.zero);
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (clickStartedEvent != null && context.started)
        {
            clickStartedEvent.Invoke();
        }
        else if (clickEndEvent != null && context.canceled)
        {
            clickEndEvent.Invoke();
        }
    }

    public void OnScan(InputAction.CallbackContext context)
    {
        if (scanEventStarted != null && context.performed)
        {
            scanEventStarted.Invoke();
        }
        else if (scanEventCanceled != null && context.canceled)
        {
            scanEventCanceled.Invoke();
        }
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        if (focusEventStarted != null && context.performed)
        {
            focusEventStarted.Invoke();
        }
        else if (focusEventCanceled != null && context.canceled)
        {
            focusEventCanceled.Invoke();
        }
    }

    public void EnablePlayerInput()
    {
        controls.Gameplay.Enable();
        controls.UI.Enable();
    }

    public void DisableAllInput()
    {
        controls.Gameplay.Disable();
        controls.UI.Disable();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    
}
