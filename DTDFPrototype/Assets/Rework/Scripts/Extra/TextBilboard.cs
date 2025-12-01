using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Get a reference to the main camera's transform
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Calculate direction to camera
        Vector3 directionToCamera = cameraTransform.position - transform.position;
        
        // Zero out the Y component to only rotate around Y axis (keep upright)
        directionToCamera.y = 0;

        // If we have a valid direction, look at the camera
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
            // Spin 180 degrees around local Y-axis to correctly display text
            transform.Rotate(0, 180, 0);
        }
    }
}