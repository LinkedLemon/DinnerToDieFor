using UnityEngine;
using UnityEngine.InputSystem; // <-- Required for the new Input System

/// <summary>
/// Spawns a prefab using the new Input System.
/// Listens for events from a PlayerInput component.
/// </summary>
public class ObjectSpawner_InputSystem : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("The 3D object prefab to spawn. Must have a Rigidbody.")]
    [SerializeField] private GameObject objectToSpawnPrefab;

    [Tooltip("The fixed Y-axis position where the object will follow the mouse.")]
    [SerializeField] private float spawnYPosition = 10f;

    // Private state variables
    private GameObject currentFollowingObject;
    private Rigidbody currentObjectRigidbody;
    private Camera mainCamera;
    private Plane spawnPlane; // A mathematical plane at our fixed Y-height

    void Awake()
    {
        mainCamera = Camera.main;
        
        // Create a plane positioned at our fixed Y-height, facing upwards
        spawnPlane = new Plane(Vector3.up, new Vector3(0, spawnYPosition, 0));
        
        if (InputManager.Instance != null && InputManager.Instance.JumpAction != null)
        {
            // Subscribe to the 'performed' event.
            // This event fires once when the button is pressed.
            InputManager.Instance.AttackAction.performed += OnDrop;
        }
    }

    void Update()
    {
        // If we are currently holding an object, update its position
        if (currentFollowingObject != null)
        {
            UpdateFollowingPosition();
        }
    }

    // --- INPUT SYSTEM EVENT HANDLERS ---
    // These methods are public so the PlayerInput component can call them.

    /// <summary>
    /// This method should be linked to your "Attack" (Left Click) Action.
    /// </summary>
    public void OnDrop(InputAction.CallbackContext context)
    {
        // We only drop if the action was "performed" (i.e., button pressed)
        // and we are actually holding an object.
        if (context.performed && currentFollowingObject != null)
        {
            DropObject();
        }
    }

    /// <summary>
    /// This method should be linked to your "Spawn" (e.g., Right Click) Action.
    /// </summary>
    public void OnSpawn(InputAction.CallbackContext context)
    {
        // Only spawn on "performed" (button pressed)
        if (context.performed)
        {
            SpawnObject();
        }
    }

    // --- CORE LOGIC ---

    public void SpawnObject()
    {
        // Don't spawn a new object if we're already holding one
        if (currentFollowingObject != null) return; 

        if (objectToSpawnPrefab == null)
        {
            Debug.LogWarning("ObjectToSpawnPrefab is not assigned in the Inspector!", this);
            return;
        }

        currentFollowingObject = Instantiate(objectToSpawnPrefab);

        ObjectSpawner_InputSystem spawner = currentFollowingObject.GetComponent<ObjectSpawner_InputSystem>();

        if (spawner != null)
        {
            Destroy(spawner);
        }

        if (!currentFollowingObject.TryGetComponent<Rigidbody>(out currentObjectRigidbody))
        {
            Debug.LogWarning("Prefab was missing a Rigidbody. Adding one automatically.", currentFollowingObject);
            currentObjectRigidbody = currentFollowingObject.AddComponent<Rigidbody>();
        }

        // "Freeze" the object
        currentObjectRigidbody.isKinematic = true;
        currentObjectRigidbody.useGravity = false;

        // Immediately update its position to the mouse
        UpdateFollowingPosition();
    }
    
    private void UpdateFollowingPosition()
    {
        // Get mouse position from the new Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        
        // Create a ray from the camera through the mouse pointer
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        // Project the ray onto our mathematical plane
        bool hit = spawnPlane.Raycast(ray, out float distance);
        Debug.Log(hit);
        if (hit)
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            Debug.Log(worldPosition);
            currentFollowingObject.transform.position = worldPosition;
        }
    }

    private void DropObject()
    {
        if (currentObjectRigidbody == null) return;

        // "Unfreeze" the object
        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useGravity = true;

        // Release our control
        currentFollowingObject = null;
        currentObjectRigidbody = null;
    }
}