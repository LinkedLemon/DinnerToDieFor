using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
    
    [Header("Events")]
    public UnityEvent OnReachedPointB;
    public UnityEvent OnReachedPointC;

    private Coroutine _activeMovement;
    
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip bellSound;
    
    private void Start()
    {
        SendOrder();
    }
    
    public void SendOrder()
    {
        tray.SetActive(true);
        
        if (_activeMovement != null)
        {
            StopCoroutine(_activeMovement);
        }
        _activeMovement = StartCoroutine(MoveObject(pointA.position, pointB.position, false, OnReachedPointB));
        SoundManager.instance.PlayAudioClip(moveSound, 1);
    }
    
    public void SubmittedOrder()
    {
        if (_activeMovement != null)
        {
            StopCoroutine(_activeMovement);
        }
        _activeMovement = StartCoroutine(MoveObject(pointB.position, pointC.position, true, OnReachedPointC));
        SoundManager.instance.PlayAudioClip(bellSound, 1);
    }
    
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, bool hideOnComplete = false, UnityEvent onComplete = null)
    {
        float journeyLength = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;

        if (journeyLength <= 0)
        {
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

        if (hideOnComplete)
        {
            tray.SetActive(false);
        }
        
        onComplete?.Invoke();

        _activeMovement = null;
    }

#if UNITY_EDITOR
    private IEnumerator DebugLoop()
    {
        if (tray == null || pointA == null || pointB == null || pointC == null)
        {
            Debug.LogError("Tray or one of its waypoints (A, B, C) is not assigned. Stopping debug loop.");
            yield break; 
        }

        tray.transform.position = pointA.position;
        tray.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            SendOrder();

            while (_activeMovement != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            SubmittedOrder();
            
            while (_activeMovement != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);
            
            tray.transform.position = pointA.position;
            tray.SetActive(true); 
            
            yield return new WaitForSeconds(1.5f);
        }
    }
#endif
}
