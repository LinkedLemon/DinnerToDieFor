using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Moves a 'tray' GameObject between three designated points (A, B, C)
/// using Lerp based on a constant speed.
/// Includes an Editor-only debug loop for testing.
/// </summary>
public class TrayPositionManager : MonoBehaviour
{
    [Header("Object to Move")]
    [Tooltip("The tray object that will be moved.")]
    [SerializeField] private GameObject tray;

    [Header("Waypoints")]
    [Tooltip("The starting point for the order (e.g., kitchen).")]
    [SerializeField] private Transform pointA;

    [Tooltip("The drop-off point for the order (e.g., customer table).")]
    [SerializeField] private Transform pointB;

    [Tooltip("The exit point where the tray goes after submission (e.g., off-screen).")]
    [SerializeField] private Transform pointC;

    [Header("Movement Settings")]
    [Tooltip("How fast the tray moves between points, in units per second.")]
    [SerializeField] private float moveSpeed = 5.0f;

    private Coroutine _activeMovement;

    /// <summary>
    /// Starts the debug loop when in the Unity Editor.
    /// </summary>
    private void Start()
    {
        // Start the debug loop only in the editor
        SendOrder();
    }

    /// <summary>
    /// Starts moving the tray from Point A to Point B.
    /// </summary>
    public void SendOrder()
    {
        Debug.Log("SendOrder: Activating tray and moving A -> B.");
        // Make sure the tray is visible before moving
        tray.SetActive(true);
        
        if (_activeMovement != null)
        {
            Debug.LogWarning("SendOrder: An existing movement was active. Stopping it.");
            StopCoroutine(_activeMovement);
        }
        _activeMovement = StartCoroutine(MoveObject(pointA.position, pointB.position));
    }

    /// <summary>
    /// Starts moving the tray from Point B to Point C, then hides it.
    /// </summary>
    public void SubmittedOrder()
    {
        Debug.Log("SubmittedOrder: Moving B -> C and hiding.");
        if (_activeMovement != null)
        {
            Debug.LogWarning("SubmittedOrder: An existing movement was active. Stopping it.");
            StopCoroutine(_activeMovement);
        }
        _activeMovement = StartCoroutine(MoveObject(pointB.position, pointC.position, true));
    }

    /// <summary>
    /// A coroutine that moves the tray from a start to an end position.
    /// </summary>
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, bool hideOnComplete = false)
    {
        Debug.Log($"MoveObject: Starting move from {startPos} to {endPos}.");
        float journeyLength = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;

        if (journeyLength <= 0)
        {
            Debug.LogWarning("MoveObject: Start and End positions are the same. Skipping move.");
            // Avoid division by zero if start and end are the same
            yield break; 
        }

        float distanceCovered = 0;
        
        while (distanceCovered < journeyLength)
        {
            distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            tray.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            
            yield return null;
        }

        // Snap to the final position to ensure accuracy
        tray.transform.position = endPos;
        Debug.Log($"MoveObject: Move finished. Snapped to {endPos}.");

        if (hideOnComplete)
        {
            tray.SetActive(false);
            Debug.Log("MoveObject: Hiding tray on complete.");
        }

        _activeMovement = null;
    }

    // --- DEBUG METHODS (Editor Only) ---

#if UNITY_EDITOR
    /// <summary>
    /// A debug coroutine to automatically cycle through the tray movements.
    /// This will only run in the Unity Editor.
    /// </summary>
    private IEnumerator DebugLoop()
    {
        Debug.Log("--- STARTING DEBUG LOOP ---");

        // Ensure all waypoints are assigned before starting
        if (tray == null || pointA == null || pointB == null || pointC == null)
        {
            Debug.LogError("Tray or one of its waypoints (A, B, C) is not assigned. Stopping debug loop.");
            yield break; // Stop the coroutine
        }

        // Initial setup
        tray.transform.position = pointA.position;
        tray.SetActive(true);
        Debug.Log("DEBUG: Tray reset to Point A.");
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            // --- 1. Send Order (A to B) ---
            Debug.Log("DEBUG: Calling SendOrder() (A -> B)");
            SendOrder();
            
            // Wait for the movement coroutine to finish (it will set _activeMovement to null)
            while (_activeMovement != null)
            {
                yield return null;
            }
            Debug.Log("DEBUG: Movement (A -> B) finished.");

            yield return new WaitForSeconds(1.5f); // Wait at point B

            // --- 2. Submit Order (B to C) ---
            Debug.Log("DEBUG: Calling SubmittedOrder() (B -> C)");
            SubmittedOrder();

            // Wait for the movement coroutine to finish
            while (_activeMovement != null)
            {
                yield return null;
            }
            Debug.Log("DEBUG: Movement (B -> C) finished and tray hidden.");

            yield return new WaitForSeconds(1.5f); // Wait while hidden

            // --- 3. Reset for next loop ---
            Debug.Log("DEBUG: Resetting tray to Point A for next loop.");
            tray.transform.position = pointA.position;
            tray.SetActive(true); 
            
            yield return new WaitForSeconds(1.5f); // Wait at point A before looping
        }
    }
#endif
}
