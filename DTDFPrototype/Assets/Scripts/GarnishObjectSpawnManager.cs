using UnityEngine;
using UnityEngine.InputSystem;

public class GarnishObjectSpawnManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("The 3D object prefabs to spawn. Must have a Rigidbody.")]
    [SerializeField] private GameObject[] garnishPrefabs;

    [Tooltip("The fixed Y-axis position where the object will follow the mouse.")]
    [SerializeField] private float spawnYPosition = 10f;

    private int currentGarnishIndex = 0;
    private GameObject currentFollowingObject;
    private Rigidbody currentObjectRigidbody;
    private Camera mainCamera;
    private Plane spawnPlane;

    void Awake()
    {
        mainCamera = Camera.main;
        spawnPlane = new Plane(Vector3.up, new Vector3(0, spawnYPosition, 0));
    }

    private void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack += OnDrop;
            InputManager.Instance.OnNext += SelectNextGarnish;
            InputManager.Instance.OnPrevious += SelectPreviousGarnish;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= OnDrop;
            InputManager.Instance.OnNext -= SelectNextGarnish;
            InputManager.Instance.OnPrevious -= SelectPreviousGarnish;
        }
    }

    void Update()
    {
        if (currentFollowingObject != null)
        {
            UpdateFollowingPosition();
        }
    }

    private void SelectNextGarnish(InputAction.CallbackContext context)
    {
        if (garnishPrefabs.Length == 0) return;
        currentGarnishIndex = (currentGarnishIndex + 1) % garnishPrefabs.Length;
        Debug.Log($"Selected Garnish: {garnishPrefabs[currentGarnishIndex].name}");
    }

    private void SelectPreviousGarnish(InputAction.CallbackContext context)
    {
        if (garnishPrefabs.Length == 0) return;
        currentGarnishIndex--;
        if (currentGarnishIndex < 0)
        {
            currentGarnishIndex = garnishPrefabs.Length - 1;
        }
        Debug.Log($"Selected Garnish: {garnishPrefabs[currentGarnishIndex].name}");
    }

    public void SpawnCurrentGarnish()
    {
        if (currentFollowingObject != null) return;
        if (garnishPrefabs.Length == 0)
        {
            Debug.LogWarning("No garnish prefabs assigned.", this);
            return;
        }

        GameObject prefabToSpawn = garnishPrefabs[currentGarnishIndex];
        currentFollowingObject = Instantiate(prefabToSpawn);

        if (!currentFollowingObject.TryGetComponent<Rigidbody>(out currentObjectRigidbody))
        {
            currentObjectRigidbody = currentFollowingObject.AddComponent<Rigidbody>();
        }

        currentObjectRigidbody.isKinematic = true;
        currentObjectRigidbody.useGravity = false;

        UpdateFollowingPosition();
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

        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useGravity = true;

        currentFollowingObject = null;
        currentObjectRigidbody = null;
    }
}
