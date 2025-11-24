using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

// This class is a serializable key-value pair for the Inspector.
[System.Serializable]
public class GarnishMapping
{
    public string garnishType;
    public GameObject garnishPrefab;
}

public class GarnishObjectSpawnManager : MonoBehaviour
{
    [Header("Garnish Settings")]
    [SerializeField]
    [Tooltip("A list that maps a string key to a garnish prefab.")]
    private List<GarnishMapping> garnishMappings;

    [Header("Spawning Settings")]
    [SerializeField]
    [Tooltip("The fixed Y-axis position where the object will follow the mouse before being dropped.")]
    private float spawnYPosition = 10f;
    [SerializeField]
    [Tooltip("Prefab for the visual indicator that shows where the garnish will land.")]
    private GameObject dropIndicatorPrefab;
    [SerializeField]
    [Tooltip("The fixed Y-axis position for the drop indicator, relative to the ground plane.")]
    private float dropIndicatorYPosition = 0.1f;

    // Runtime dictionary for fast lookups.
    private Dictionary<string, GameObject> garnishDictionary;

    // State for the object being moved
    private GameObject currentFollowingObject;
    private Rigidbody currentObjectRigidbody;
    private Camera mainCamera;
    private Plane spawnPlane;

    // State for the drop indicator
    private GameObject currentDropIndicator;

    private void Awake()
    {
        mainCamera = Camera.main;
        spawnPlane = new Plane(Vector3.up, new Vector3(0, spawnYPosition, 0));

        garnishDictionary = new Dictionary<string, GameObject>();
        foreach (var mapping in garnishMappings)
        {
            if (mapping != null && !string.IsNullOrEmpty(mapping.garnishType) && mapping.garnishPrefab != null)
            {
                if (!garnishDictionary.ContainsKey(mapping.garnishType))
                {
                    garnishDictionary.Add(mapping.garnishType, mapping.garnishPrefab);
                }
                else
                {
                    Debug.LogWarning($"Duplicate garnish type '{mapping.garnishType}' found. The first one will be used.", this);
                }
            }
        }
    }

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            // Subscribe to the release event for the drop
            InputManager.Instance.OnAttackCanceled += OnDrop;
        }
        else
        {
            Debug.LogError("GarnishObjectSpawnManager: InputManager.Instance is null.", this);
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttackCanceled -= OnDrop;
        }
    }

    private void Update()
    {
        if (currentFollowingObject != null)
        {
            UpdateFollowingPosition();
        }
    }

    /// <summary>
    /// Spawns a garnish and holds it, following the mouse, until it is dropped.
    /// </summary>
    public void SpawnRequestedGarnish(string garnishType)
    {
        if (currentFollowingObject != null)
        {
            Debug.LogWarning("Already holding a garnish. Drop it first before spawning a new one.", this);
            return;
        }

        if (garnishDictionary.TryGetValue(garnishType, out GameObject prefabToSpawn))
        {
            currentFollowingObject = Instantiate(prefabToSpawn);

            if (!currentFollowingObject.TryGetComponent<Rigidbody>(out currentObjectRigidbody))
            {
                currentObjectRigidbody = currentFollowingObject.AddComponent<Rigidbody>(); // Fixed: use currentFollowingObject here
            }

            currentObjectRigidbody.isKinematic = true;
            currentObjectRigidbody.useGravity = false;

            // Instantiate and position the drop indicator
            if (dropIndicatorPrefab != null)
            {
                currentDropIndicator = Instantiate(dropIndicatorPrefab);
                // Position indicator at the same X-Z as the garnish, but at its fixed Y
                currentDropIndicator.transform.position = new Vector3(currentFollowingObject.transform.position.x, dropIndicatorYPosition, currentFollowingObject.transform.position.z);
            }
            else
            {
                Debug.LogWarning("Drop Indicator Prefab is not assigned in GarnishObjectSpawnManager. Cannot show drop indicator.", this);
            }


            Debug.Log($"Spawning and holding garnish: {garnishType}");
            UpdateFollowingPosition(); // Position it correctly right away
        }
        else
        {
            Debug.LogError($"Failed to spawn garnish. Type '{garnishType}' not found.", this);
        }
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        if (currentFollowingObject != null)
        {
            DropObject();
        }
    }

    private void UpdateFollowingPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        if (spawnPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            currentFollowingObject.transform.position = worldPosition;

            // Update drop indicator position
            if (currentDropIndicator != null)
            {
                currentDropIndicator.transform.position = new Vector3(worldPosition.x, dropIndicatorYPosition, worldPosition.z);
            }
        }
    }

    private void DropObject()
    {
        if (currentObjectRigidbody == null) return;

        Debug.Log("Dropping garnish.");
        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useGravity = true;

        // Destroy the drop indicator
        if (currentDropIndicator != null)
        {
            Destroy(currentDropIndicator);
            currentDropIndicator = null;
        }

        // Release control
        currentFollowingObject = null;
        currentObjectRigidbody = null;
    }
}
