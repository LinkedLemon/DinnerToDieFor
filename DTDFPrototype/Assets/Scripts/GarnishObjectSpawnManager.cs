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

    // Runtime dictionary for fast lookups.
    private Dictionary<string, GameObject> garnishDictionary;

    // State for the object being moved
    private GameObject currentFollowingObject;
    private Rigidbody currentObjectRigidbody;
    private Camera mainCamera;
    private Plane spawnPlane;

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
            InputManager.Instance.OnAttack += OnDrop;
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
            InputManager.Instance.OnAttack -= OnDrop;
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
                currentObjectRigidbody = currentObjectRigidbody.AddComponent<Rigidbody>();
            }

            currentObjectRigidbody.isKinematic = true;
            currentObjectRigidbody.useGravity = false;

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
        }
    }

    private void DropObject()
    {
        if (currentObjectRigidbody == null) return;

        Debug.Log("Dropping garnish.");
        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useGravity = true;

        // Release control
        currentFollowingObject = null;
        currentObjectRigidbody = null;
    }
}
