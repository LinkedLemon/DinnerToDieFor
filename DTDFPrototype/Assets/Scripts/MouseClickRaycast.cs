using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events; // Import the UnityEvent system

/// <summary>
/// Performs a raycast on mouse click and broadcasts the hit information
/// via a UnityEvent.
/// </summary>
public class MouseClickRaycast : MonoBehaviour
{
    // This event will broadcast the full RaycastHit details to any listener.
    // We make it public so the Communicator can listen to it.
    [Header("Events")]
    public UnityEvent<RaycastHit> OnRaycastHit;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MouseClickRaycast: No main camera found! Please tag your camera 'MainCamera'.");
        }
    }

    void Update()
    {
        // Ensure camera and mouse are valid
        if (mainCamera == null) return;
        
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        // Get the mouse position
        Vector2 mousePosition = mouse.position.ReadValue();

        // Create the ray
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        // Perform the raycast
        if (Physics.Raycast(ray, out hitInfo))
        {
            // A hit was detected!
            string objectName = hitInfo.transform.name;
            string objectTag = hitInfo.transform.tag;
            Debug.Log($"Raycast Hit: {objectName}, Tag: {objectTag}");

            // Invoke the event and pass the hit data to all listeners.
            OnRaycastHit.Invoke(hitInfo);
        }
        else
        {
            Debug.Log("Raycast clicked on empty space.");
        }
    }
}