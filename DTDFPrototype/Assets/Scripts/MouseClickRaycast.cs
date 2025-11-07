using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events; // Import the UnityEvent system

public class MouseClickRaycast : MonoBehaviour
{
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
        if (mainCamera == null) return;
        
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Vector2 mousePosition = mouse.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            string objectName = hitInfo.transform.name;
            string objectTag = hitInfo.transform.tag;
            Debug.Log($"Raycast Hit: {objectName}, Tag: {objectTag}");

            OnRaycastHit.Invoke(hitInfo);
        }
        else
        {
            Debug.Log("Raycast clicked on empty space.");
        }
    }
}