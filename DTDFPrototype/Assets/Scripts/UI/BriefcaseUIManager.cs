using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BriefcaseUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private RawImage displayImage;

    [Header("Image Data")]
    [SerializeField] private List<Texture> imagesToCycle;

    private int _currentIndex = 0;

    void Start()
    {
        // Add listeners to the buttons
        leftButton.onClick.AddListener(CycleLeft);
        rightButton.onClick.AddListener(CycleRight);
        closeButton.onClick.AddListener(HideUI);

        // Initially hide the canvas
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        leftButton.onClick.RemoveListener(CycleLeft);
        rightButton.onClick.RemoveListener(CycleRight);
        closeButton.onClick.RemoveListener(HideUI);
    }

    /// <summary>
    /// Shows the briefcase UI.
    /// </summary>
    public void ShowUI()
    {
        if (canvasObject != null)
        {
            _currentIndex = 0;
            UpdateImage();
            canvasObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Canvas object is not assigned in the BriefcaseUIManager.");
        }
    }

    /// <summary>
    /// Hides the briefcase UI.
    /// </summary>
    public void HideUI()
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private void CycleLeft()
    {
        if (imagesToCycle == null || imagesToCycle.Count == 0) return;

        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = imagesToCycle.Count - 1;
        }
        UpdateImage();
    }

    private void CycleRight()
    {
        if (imagesToCycle == null || imagesToCycle.Count == 0) return;

        _currentIndex++;
        if (_currentIndex >= imagesToCycle.Count)
        {
            _currentIndex = 0;
        }
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (imagesToCycle != null && imagesToCycle.Count > 0 && displayImage != null)
        {
            displayImage.texture = imagesToCycle[_currentIndex];
        }
        else
        {
            Debug.LogError("Image list or display image is not set up correctly in the BriefcaseUIManager.");
        }
    }
}
