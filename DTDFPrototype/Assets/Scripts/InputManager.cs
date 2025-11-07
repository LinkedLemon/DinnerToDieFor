using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputSystem_Actions inputActions;

    // --- Player Action Events ---
    public event Action<InputAction.CallbackContext> OnMove;
    public event Action<InputAction.CallbackContext> OnLook;
    public event Action<InputAction.CallbackContext> OnAttack;
    public event Action<InputAction.CallbackContext> OnInteract;
    public event Action<InputAction.CallbackContext> OnCrouch;
    public event Action<InputAction.CallbackContext> OnJump;
    public event Action<InputAction.CallbackContext> OnPrevious;
    public event Action<InputAction.CallbackContext> OnNext;
    public event Action<InputAction.CallbackContext> OnSprint;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
    
            inputActions = new InputSystem_Actions();
            
            inputActions.Player.Move.performed += ctx => OnMove?.Invoke(ctx);
            inputActions.Player.Look.performed += ctx => OnLook?.Invoke(ctx);
            inputActions.Player.Attack.performed += ctx => OnAttack?.Invoke(ctx);
            inputActions.Player.Interact.performed += ctx => OnInteract?.Invoke(ctx);
            inputActions.Player.Crouch.performed += ctx => OnCrouch?.Invoke(ctx);
            inputActions.Player.Jump.performed += ctx => OnJump?.Invoke(ctx);
            inputActions.Player.Previous.performed += ctx => OnPrevious?.Invoke(ctx);
            inputActions.Player.Next.performed += ctx => OnNext?.Invoke(ctx);
            inputActions.Player.Sprint.performed += ctx => OnSprint?.Invoke(ctx);
        }
    
        private void OnEnable()
        {
            inputActions.Player.Enable();
        }
    
        private void OnDisable()
        {
            inputActions.Player.Disable();
        }
        
        public Vector2 GetMoveVector()
        {
            return inputActions.Player.Move.ReadValue<Vector2>();
        }
    public Vector2 GetLookVector()
    {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }
}
