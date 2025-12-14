using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CameraControlManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator cameraAnimator;
    [Tooltip("The RawImage area at the top of the screen (to look up/at customers).")]
    [SerializeField] private Image lookUpImage;
    [Tooltip("The RawImage area at the bottom of the screen (to look down/at tray).")]
    [SerializeField] private Image lookDownImage;

    [Header("Animation Settings")]
    [SerializeField] private string lookUpTrigger = "LookUp";
    [SerializeField] private string lookDownTrigger = "LookDown";
    [Tooltip("Time in seconds for the animation to complete before showing the next button.")]
    [SerializeField] private float animationDuration = 1.0f;

    // Helper class to handle hover events on non-Button UI elements
    public class SimpleHoverListener : MonoBehaviour, IPointerEnterHandler
    {
        public System.Action onHover;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHover?.Invoke();
        }
    }

    private void Start()
    {
        // Validate references
        if (cameraAnimator == null) Debug.LogError("CameraControlManager: Camera Animator is not assigned!");
        if (lookUpImage == null) Debug.LogError("CameraControlManager: Look Up Image is not assigned!");
        if (lookDownImage == null) Debug.LogError("CameraControlManager: Look Down Image is not assigned!");

        // Setup hover listeners
        SetupImageListener(lookUpImage, OnLookUpHover);
        SetupImageListener(lookDownImage, OnLookDownHover);

        // Initial Setup: Assuming we start looking DOWN at the tray.
        // Therefore, we can Look Up.
        if (lookUpImage != null)
        {
            lookUpImage.gameObject.SetActive(false);
        }

        if (lookDownImage != null)
        {
            lookDownImage.gameObject.SetActive(true);
        }
    }

    private void SetupImageListener(Image image, System.Action callback)
    {
        if (image != null)
        {
            // Add the listener component if it doesn't exist
            SimpleHoverListener listener = image.gameObject.GetComponent<SimpleHoverListener>();
            if (listener == null)
            {
                listener = image.gameObject.AddComponent<SimpleHoverListener>();
            }
            listener.onHover = callback;
        }
    }

    public void TriggerLookUp()
    {
        OnLookUpHover();
    }

    public void TriggerLookDown()
    {
        OnLookDownHover();
    }

    private void OnLookUpHover()
    {
        // Play Animation
        if (cameraAnimator != null)
        {
            cameraAnimator.ResetTrigger(lookDownTrigger);
            cameraAnimator.SetTrigger(lookUpTrigger);
        }

        // Hide this button immediately
        if (lookUpImage != null) lookUpImage.gameObject.SetActive(false);

        // Enable the other button after delay
        StartCoroutine(EnableImageAfterDelay(lookDownImage, animationDuration));
    }

    private void OnLookDownHover()
    {
        // Play Animation
        if (cameraAnimator != null)
        {
            cameraAnimator.ResetTrigger(lookUpTrigger);
            cameraAnimator.SetTrigger(lookDownTrigger);
        }

        // Hide this button immediately
        if (lookDownImage != null) lookDownImage.gameObject.SetActive(false);

        // Enable the other button after delay
        StartCoroutine(EnableImageAfterDelay(lookUpImage, animationDuration));
    }

    private IEnumerator EnableImageAfterDelay(Image imageToEnable, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (imageToEnable != null)
        {
            imageToEnable.gameObject.SetActive(true);
        }
    }
}
