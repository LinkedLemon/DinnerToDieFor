using UnityEngine;

/// <summary>
/// An example responder script.
/// Listens for a RaycastHit event. If the hit object has a specific tag,
/// this script will change the material of a separate target object.
/// </summary>
public class MaterialChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Tooltip("The tag the raycast must hit to trigger the change.")]
    private string triggerTag;

    [SerializeField]
    [Tooltip("The GameObject whose material will be changed.")]
    private GameObject objectToChange;

    [SerializeField]
    [Tooltip("The new material to apply to the target object.")]
    private Material newMaterial;

    /// <summary>
    /// This public method will be connected to the
    /// 'RaycastEventCommunicator.OnHitRelayed' event in the Inspector.
    /// </summary>
    public void CheckHitTagAndChangeTarget(RaycastHit hit)
    {
        // Check 1: Do we have a material to apply?
        if (newMaterial == null)
        {
            Debug.LogWarning("MaterialChanger: No 'newMaterial' assigned.", this);
            return;
        }

        // Check 2: Do we have an object to change?
        if (objectToChange == null)
        {
            Debug.LogWarning("MaterialChanger: No 'objectToChange' assigned.", this);
            return;
        }
        
        // Check 3: Is the trigger tag set?
        if (string.IsNullOrEmpty(triggerTag))
        {
            Debug.LogWarning("MaterialChanger: No 'triggerTag' assigned.", this);
            return;
        }

        // Check 4: Does the hit object's tag match our trigger tag?
        if (hit.transform.CompareTag(triggerTag))
        {
            // Tag matches!
            Renderer targetRenderer = objectToChange.GetComponent<Renderer>();
            
            if (targetRenderer != null)
            {
                // Apply the new material to the target object
                Debug.Log($"Hit '{hit.transform.name}' with tag '{triggerTag}'. Changing material on '{objectToChange.name}'.");
                targetRenderer.material = newMaterial;
            }
            else
            {
                Debug.LogWarning($"MaterialChanger: The target object '{objectToChange.name}' has no Renderer component.", objectToChange);
            }
        }
        // If the tag doesn't match, this script simply does nothing.
    }
}

