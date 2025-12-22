using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void MouseInteraction(Vector3 position);

    public static event MouseInteraction PrimaryInteraction;
    public static event MouseInteraction SecondaryInteraction;
    
    private static InputManager _instance;
    private PlayerInputStandard playerControls;

    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Double InputManager");
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        
        
        playerControls = new PlayerInputStandard();

        playerControls.Standard.PrimaryInteract.performed += SignalPrimaryInteraction;
        playerControls.Standard.SecondaryInteract.performed += SignalSecondaryInteraction;
        
        Debug.Log(_instance);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public static void SetActive(bool value)
    {
        _instance.enabled = value;
    }

    public static Vector2 GetPlayerMovement()
    {
        return _instance.playerControls.Standard.Movement.ReadValue<Vector2>();
    }

    public static Vector3 GetMousePosition()
    {
        return _instance.playerControls.Standard.MousePosition.ReadValue<Vector2>();
    }

    private void SignalPrimaryInteraction(InputAction.CallbackContext context)
    {
        PrimaryInteraction?.Invoke(GetMousePosition());
    }

    private void SignalSecondaryInteraction(InputAction.CallbackContext context)
    {
        SecondaryInteraction?.Invoke(GetMousePosition());
    }

}