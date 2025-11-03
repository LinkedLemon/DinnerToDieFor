using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // <-- Required for the new Input System
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
    private Plane spawnPlane;

    void Awake()
    {
        mainCamera = Camera.main;
        
        spawnPlane = new Plane(Vector3.up, new Vector3(0, spawnYPosition, 0));
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.AttackAction != null)
        {
            BindAction();
        }
        else
        {
            StartCoroutine(BindWhenReady());
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.AttackAction != null)
        {
            InputManager.Instance.AttackAction.performed -= OnDrop;
            Debug.Log("Unbinded attack from drop");
        }
    }

    private IEnumerator BindWhenReady()
    {
        yield return new WaitUntil(() => InputManager.Instance != null && InputManager.Instance.AttackAction != null);
        
        BindAction();
    }

    private void BindAction()
    {
        InputManager.Instance.AttackAction.performed -= OnDrop;
        InputManager.Instance.AttackAction.performed += OnDrop;
        
        Debug.Log("Binded attack to drop");
    }
    
    void Update()
    {
        if (currentFollowingObject != null)
        {
            UpdateFollowingPosition();
        }
    }
    
    private void OnDrop(InputAction.CallbackContext context)
    {
        Debug.Log("Dropping object");
        if (context.performed && currentFollowingObject != null)
        {
            DropObject();
        }
    }

    // --- CORE LOGIC ---

    public void SpawnObject()
    {
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
        
        currentObjectRigidbody.isKinematic = true;
        currentObjectRigidbody.useGravity = false;
        
        UpdateFollowingPosition();
    }
    
    private void UpdateFollowingPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        
        if (spawnPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            Debug.Log(worldPosition);
            currentFollowingObject.transform.position = worldPosition;
        }
    }

    private void DropObject()
    {
        if (currentObjectRigidbody == null) return;
        
        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useGravity = true;
        
        currentFollowingObject = null;
        currentObjectRigidbody = null;
    }
}