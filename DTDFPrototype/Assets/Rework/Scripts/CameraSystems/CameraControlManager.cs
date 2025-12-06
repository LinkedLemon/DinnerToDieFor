using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraControlManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator cameraAnimator;
    [Tooltip("The button at the top of the screen (to look up/at customers).")]
    [SerializeField] private Button lookUpButton;
    [Tooltip("The button at the bottom of the screen (to look down/at tray).")]
    [SerializeField] private Button lookDownButton;

    [Header("Animation Settings")]
    [SerializeField] private string lookUpTrigger = "LookUp";
    [SerializeField] private string lookDownTrigger = "LookDown";
    [Tooltip("Time in seconds for the animation to complete before showing the next button.")]
    [SerializeField] private float animationDuration = 1.0f;

    private void Start()
    {
        // Validate references
        if (cameraAnimator == null) Debug.LogError("CameraControlManager: Camera Animator is not assigned!");
        if (lookUpButton == null) Debug.LogError("CameraControlManager: Look Up Button is not assigned!");
        if (lookDownButton == null) Debug.LogError("CameraControlManager: Look Down Button is not assigned!");

        // Initial Setup: Assuming we start looking DOWN at the tray.
        // Therefore, we can Look Up.
        if (lookUpButton != null)
        {
            lookUpButton.onClick.AddListener(OnLookUpClicked);
            lookUpButton.gameObject.SetActive(false);
        }

        if (lookDownButton != null)
        {
            lookDownButton.onClick.AddListener(OnLookDownClicked);
            lookDownButton.gameObject.SetActive(true);
        }
    }

    public void TriggerLookUp()
    {
        OnLookUpClicked();
    }

    public void TriggerLookDown()
    {
        OnLookDownClicked();
    }

    private void OnLookUpClicked()
    {
        // Play Animation
        if (cameraAnimator != null)
        {
            cameraAnimator.ResetTrigger(lookDownTrigger);
            cameraAnimator.SetTrigger(lookUpTrigger);
        }

        // Hide this button immediately
        if (lookUpButton != null) lookUpButton.gameObject.SetActive(false);

        // Enable the other button after delay
        StartCoroutine(EnableButtonAfterDelay(lookDownButton, animationDuration));
    }

    private void OnLookDownClicked()
    {
        // Play Animation
        if (cameraAnimator != null)
        {
            cameraAnimator.ResetTrigger(lookUpTrigger);
            cameraAnimator.SetTrigger(lookDownTrigger);
        }

        // Hide this button immediately
        if (lookDownButton != null) lookDownButton.gameObject.SetActive(false);

        // Enable the other button after delay
        StartCoroutine(EnableButtonAfterDelay(lookUpButton, animationDuration));
    }

    private IEnumerator EnableButtonAfterDelay(Button buttonToEnable, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (buttonToEnable != null)
        {
            buttonToEnable.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (lookUpButton != null) lookUpButton.onClick.RemoveListener(OnLookUpClicked);
        if (lookDownButton != null) lookDownButton.onClick.RemoveListener(OnLookDownClicked);
    }
}
