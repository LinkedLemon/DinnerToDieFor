using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AllergyDisplayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data Binding")]
    [Tooltip("List of TextMeshPro components for Customer Names, corresponding to the 5 seats.")]
    [SerializeField] private List<TextMeshProUGUI> nameTexts;
    [Tooltip("List of TextMeshPro components for Allergy Names, corresponding to the 5 seats.")]
    [SerializeField] private List<TextMeshProUGUI> allergyTexts;

    [Header("Animation Settings")]
    [Tooltip("The RectTransform of the paper/panel to animate.")]
    [SerializeField] private RectTransform paperRect;
    
    [Tooltip("The vertical distance to slide up when hovered.")]
    [SerializeField] private float slideUpHeight = 200f;
    
    [Tooltip("Speed of the slide animation.")]
    [SerializeField] private float animationSpeed = 10f;

    private Vector2 _hiddenPosition;
    private Vector2 _shownPosition;
    private Vector2 _targetPosition;
    private bool _initialized = false;

    private void Start()
    {
        if (paperRect == null) paperRect = GetComponent<RectTransform>();
        
        if (paperRect != null)
        {
            _hiddenPosition = paperRect.anchoredPosition;
            _shownPosition = _hiddenPosition + new Vector2(0, slideUpHeight);
            _targetPosition = _hiddenPosition;
            _initialized = true;
        }
        else
        {
            Debug.LogError("[AllergyDisplayUI] No RectTransform assigned or found!");
        }

        // Subscribe to round updates to refresh data
        if (RoundManager.Instance != null)
        {
            if (RoundManager.Instance.OnNewRoundStarted != null)
            {
                RoundManager.Instance.OnNewRoundStarted.AddListener(RefreshDisplay);
            }
            // Also try initial refresh
            RefreshDisplay();
        }
    }

    private void Update()
    {
        if (_initialized)
        {
            paperRect.anchoredPosition = Vector2.Lerp(paperRect.anchoredPosition, _targetPosition, Time.deltaTime * animationSpeed);
        }
    }

    public void RefreshDisplay()
    {
        if (RoundManager.Instance == null) return;
        
        var customers = RoundManager.Instance.ActiveCustomers;
        
        for (int i = 0; i < nameTexts.Count; i++)
        {
            if (nameTexts[i] == null || allergyTexts[i] == null) continue;

            if (i < customers.Count)
            {
                CustomerRuntimeData customer = customers[i];
                nameTexts[i].text = customer.Data.CustomerName;
                allergyTexts[i].text = customer.CurrentAllergy.ToString();
            }
            else
            {
                // Clear slots if fewer customers than texts
                nameTexts[i].text = "";
                allergyTexts[i].text = "";
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_initialized)
        {
            _targetPosition = _shownPosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_initialized)
        {
            _targetPosition = _hiddenPosition;
        }
    }
}
