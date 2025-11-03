using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Acts as a central communicator (middle-man).
/// It listens to a raycaster and relays the hit event to other systems.
/// This allows you to easily swap raycasters or responders without
/// breaking connections.
/// </summary>
public class RaycastEventCommunicator : MonoBehaviour
{
    [Header("Input (Source)")]
    [SerializeField]
    [Tooltip("The raycast script we are listening to.")]
    private MouseClickRaycast raycastSource;

    [Header("Output (Relay)")]
    [Tooltip("The event this communicator will fire to relay the data.")]
    public UnityEvent<RaycastHit> OnHitRelayed;

    private void Awake()
    {
        if (raycastSource == null)
        {
            Debug.LogError("RaycastEventCommunicator: No 'raycastSource' assigned! Please assign it in the Inspector.", this);
            return;
        }

        // Subscribe our 'RelayHit' method to the raycaster's event.
        // Now, when raycastSource.OnRaycastHit fires, RelayHit will be called.
        raycastSource.OnRaycastHit.AddListener(RelayHit);
    }

    private void OnDestroy()
    {
        // Always good practice to unsubscribe when this object is destroyed
        // to prevent errors.
        if (raycastSource != null)
        {
            raycastSource.OnRaycastHit.RemoveListener(RelayHit);
        }
    }

    /// <summary>
    /// This method is called by the raycastSource's event.
    /// </summary>
    private void RelayHit(RaycastHit hit)
    {
        // This object can do its own logic here first if needed
        // (e.g., log the hit, filter it, etc.).
        
        // Now, fire our *own* event to pass the data along
        // to any final listeners (like the MaterialChanger).
        OnHitRelayed.Invoke(hit);
    }
}