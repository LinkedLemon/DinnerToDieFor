using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

/// <summary>
/// A generic responder that listens for a RaycastHit event.
/// If the hit object has the desired tag, this script
/// broadcasts its own UnityEvent, passing along the hit info.
/// </summary>
public class HitTagEventRelay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Tooltip("The tag the raycast must hit to trigger the event.")]
    private string triggerTag;

    [Header("Events")]
    [Tooltip("This event is broadcast when a hit is received AND its tag matches the 'Trigger Tag'.")]
    public UnityEvent<RaycastHit> OnTagMatched; // Public to be assigned in Inspector

    /// <summary>
    /// This public method should be connected to your
    /// 'RaycastEventCommunicator.OnHitRelayed' (or similar) event.
    /// It checks the tag of the hit object and relays the event if it matches.
    /// </summary>
    public void CheckHitTag(RaycastHit hit)
    {
        // Check 1: Is the trigger tag set?
        if (string.IsNullOrEmpty(triggerTag))
        {
            Debug.LogWarning("HitTagEventRelay: No 'triggerTag' assigned.", this);
            return;
        }

        // Check 2: Does the hit object's tag match our trigger tag?
        if (hit.transform.CompareTag(triggerTag))
        {
            // Tag matches! Broadcast the event to all listeners.
            Debug.Log($"Hit '{hit.transform.name}' with tag '{triggerTag}'. Invoking OnTagMatched event.");
            OnTagMatched?.Invoke(hit);
        }
        // If the tag doesn't match, this script simply does nothing.
    }
}