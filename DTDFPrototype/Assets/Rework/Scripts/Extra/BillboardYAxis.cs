using UnityEngine;

[ExecuteInEditMode] // Optional: for previewing in editor
public class BillboardYAxis : MonoBehaviour
{
    [SerializeField]
    private Transform _targetCamera; // Assign your main camera here

    // Use Camera.main if _targetCamera is not assigned
    private void Start()
    {
        if (_targetCamera == null)
        {
            _targetCamera = Camera.main?.transform;
            if (_targetCamera == null)
            {
                Debug.LogWarning("BillboardYAxis: No target camera assigned and Camera.main not found. Please assign a camera.");
                enabled = false; // Disable script if no camera
            }
        }
    }

    void LateUpdate()
    {
        if (_targetCamera == null) return;

        // Get the direction from this object to the camera
        Vector3 directionToCamera = _targetCamera.position - transform.position;

        // Project the direction onto the XZ plane to ignore Y-axis tilt
        directionToCamera.y = 0;

        // Ensure the direction is not zero (e.g., if camera and object are at the same XZ position)
        if (directionToCamera == Vector3.zero) return;

        // Calculate the rotation to look at the projected direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // Apply only the Y-axis rotation from the targetRotation
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x, // Keep original X
            targetRotation.eulerAngles.y,     // Use calculated Y
            transform.rotation.eulerAngles.z  // Keep original Z
        );
    }
}
