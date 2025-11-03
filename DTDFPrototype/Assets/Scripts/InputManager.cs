using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // --- SINGLETON PATTERN ---
    public static InputManager Instance { get; private set; }

    // --- INPUT ACTIONS ---
    
    [Header("Input Asset")]
    [Tooltip("Drag your main .inputactions asset here.")]
    [SerializeField]
    private InputActionAsset inputActionAsset;

    // --- PUBLIC ACTION PROPERTIES ---
    // These are the public properties other scripts will use to
    // read inputs. We find and assign these in Awake().
    public InputActionMap PlayerActions { get; private set; }
    
    public InputAction MoveAction { get; private set; }
    
    public InputAction JumpAction { get; private set; }
    
    public InputAction LookAction { get; private set; }
    
    public InputAction AttackAction { get; private set; }

    // (Add more public InputAction properties here for any other actions
    // you want to expose, like "Fire", "Interact", etc.)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate InputManager found. Destroying new instance.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        // --- INPUT SYSTEM SETUP ---

        if (inputActionAsset == null)
        {
            Debug.LogError("InputManager: InputActionAsset is not assigned in the Inspector!");
            return;
        }

        // Find and cache the Action Maps and Actions by name.
        // IMPORTANT: These strings ("Player", "Move", "Jump", "Look")
        // MUST match the names you gave them in the Input Action Asset editor.

        PlayerActions = inputActionAsset.FindActionMap("Player");
        if (PlayerActions == null)
        {
            Debug.LogError("InputManager: Could not find 'Player' Action Map in the asset.");
            return;
        }

        // Find specific actions within the "Player" map
        MoveAction = PlayerActions.FindAction("Move");
        JumpAction = PlayerActions.FindAction("Jump");
        LookAction = PlayerActions.FindAction("Look");
        AttackAction = PlayerActions.FindAction("Attack");

        // Log warnings if any actions weren't found (helps with debugging typos)
        if (MoveAction == null) Debug.LogWarning("InputManager: 'Move' action not found.");
        if (JumpAction == null) Debug.LogWarning("InputManager: 'Jump' action not found.");
        if (LookAction == null) Debug.LogWarning("InputManager: 'Look' action not found.");

        // (Add more 'Find' calls here for your other actions)
    }

    /// <summary>
    /// Enable all our relevant action maps when this object is enabled.
    /// </summary>
    private void OnEnable()
    {
        // Enabling the map enables all actions within it.
        PlayerActions?.Enable();
        
        // You could also enable other maps here if you have them
        // e.g., UIActions?.Enable();
    }

    /// <summary>
    /// Disable all our action maps when this object is disabled.
    /// </summary>
    private void OnDisable()
    {
        PlayerActions?.Disable();
        
        // e.g., UIActions?.Disable();
    }
}
