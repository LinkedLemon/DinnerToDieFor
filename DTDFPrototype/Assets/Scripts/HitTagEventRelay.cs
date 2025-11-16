using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Listens to a RaycastEventCommunicator and checks if the hit
/// object's layer and/or tag match specified criteria.
/// If they match, it broadcasts its own UnityEvent.
/// </summary>
public class HitTagEventRelay : MonoBehaviour
{
    [Header("Input (Source)")]
    [SerializeField]
    [Tooltip("The event communicator we are listening to.")]
    private RaycastEventCommunicator eventSource;

    [Header("Filtering Criteria")]
    [SerializeField]
    [Tooltip("The layer the hit object must be on to trigger the event.")]
    private LayerMask triggerLayer;

    [SerializeField]
    [Tooltip("The tag the hit object must have. If empty or null, this check is ignored.")]
    private string triggerTag;

    [Header("Output Event")]
    [Tooltip("This event is broadcast when a hit is received AND its layer/tag match the criteria.")]
    public UnityEvent<string> OnMatch;

    private void Awake()
    {
        if (eventSource == null)
        {
            Debug.LogError("HitTagEventRelay: No 'eventSource' assigned! Please assign it in the Inspector.", this);
            return;
        }
        eventSource.OnHitRelayed.AddListener(HandleHit);
    }

    private void OnDestroy()
    {
        if (eventSource != null)
        {
            eventSource.OnHitRelayed.RemoveListener(HandleHit);
        }
    }

    /// <summary>
    /// Receives hit info and checks if it meets the filtering criteria.
    /// </summary>
    private void HandleHit(int layer, string tag)
    {
        // Check 1: Does the layer match?
        // We use a bitwise operation to see if the hit layer is part of our LayerMask.
        bool layerMatches = (triggerLayer.value & (1 << layer)) != 0;

        if (!layerMatches)
        {
            return; // Layer doesn't match, no need to continue.
        }

        // Check 2: Do we need to check the tag?
        bool tagCheckRequired = !string.IsNullOrEmpty(triggerTag);

        if (tagCheckRequired)
        {
            // If tag check is required, and it doesn't match, then fail.
            if (tag != triggerTag)
            {
                return;
            }
        }

        // If we've reached this point, all required checks have passed.
        Debug.Log($"Hit matched criteria (Layer: {LayerMask.LayerToName(layer)}, Tag: {tag}). Invoking OnMatch event.");
        OnMatch?.Invoke(tag);
    }
}
