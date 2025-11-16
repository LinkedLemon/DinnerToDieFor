using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MouseClickRaycast : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<int, string> OnRaycastHit;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MouseClickRaycast: No main camera found! Please tag your camera 'MainCamera'.");
        }

        // Subscribe to the OnAttack event from the InputManager
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack += PerformRaycast;
        }
        else
        {
            Debug.LogError("MouseClickRaycast: InputManager.Instance is null. Make sure an InputManager is in the scene.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= PerformRaycast;
        }
    }

    private void PerformRaycast(InputAction.CallbackContext context)
    {
        if (mainCamera == null) return;

        // We only care about the "performed" phase of the action (the click itself)
        if (!context.performed) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            int objectLayer = hitInfo.collider.gameObject.layer;
            string objectTag = hitInfo.collider.tag;
            Debug.Log($"Raycast Hit: Layer {objectLayer}, Tag: {objectTag}");

            OnRaycastHit.Invoke(objectLayer, objectTag);
        }
        else
        {
            Debug.Log("Raycast clicked on empty space.");
        }
    }
}
