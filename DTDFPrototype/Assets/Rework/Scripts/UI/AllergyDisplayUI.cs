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
    
    [Tooltip("List of Allergy Grid Buttons. Should be 25 buttons (5 customers * 5 garnishes).")]
    [SerializeField] private List<AllergyGridButton> gridButtons;

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
        bool isNewGame = RoundManager.Instance.RoundCount == 1;
        
        // Define the column order strictly matching the expected UI layout:
        // Columns (Left to Right): Paprika, Olives, Cherry, Thyme, Cocktail Sword
        List<GarnishEnums.GarnishType> garnishColumns = new List<GarnishEnums.GarnishType>
        {
            GarnishEnums.GarnishType.paprika,
            GarnishEnums.GarnishType.olive,
            GarnishEnums.GarnishType.cherry,
            GarnishEnums.GarnishType.thyme,
            GarnishEnums.GarnishType.sword // Assuming 'cocktail sword' maps to 'sword' enum
        };

        int garnishCount = garnishColumns.Count;
        int customerCount = nameTexts.Count; // Assuming nameTexts count matches active customer count (5)

        for (int i = 0; i < customerCount; i++) // Customers (Rows)
        {
            if (nameTexts[i] == null) continue;

            CustomerRuntimeData customer = (i < customers.Count) ? customers[i] : null;

            // Update Name
            if (customer != null)
            {
                nameTexts[i].text = customer.Data.CustomerName;
            }
            else
            {
                nameTexts[i].text = "";
            }

            // Update Grid Buttons for this customer (Row i)
            for (int j = 0; j < garnishCount; j++) // Garnishes (Columns)
            {
                // Column-major order: iterate down each column first, then move to the next column.
                int buttonIndex = j * customerCount + i; 
                if (buttonIndex >= gridButtons.Count)
                {
                    Debug.LogWarning($"Button index {buttonIndex} out of bounds for gridButtons. Ensure gridButtons list is populated correctly (Expected: {garnishCount * customerCount}, Actual: {gridButtons.Count}).");
                    break;
                }

                AllergyGridButton button = gridButtons[buttonIndex];
                if (button == null) continue;

                if (customer == null)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }
                
                button.gameObject.SetActive(true);

                if (isNewGame)
                {
                    button.ResetToDefault();
                }

                if (!customer.IsAlive)
                {
                    // Lock entire row to Dead if customer is dead
                    button.SetLockedState(true, false);
                }
                else
                {
                    // Check if this column corresponds to the customer's allergy
                    GarnishEnums.GarnishType columnType = garnishColumns[j];
                    
                    if (customer.CurrentAllergy == columnType)
                    {
                        // Lock to Allergy Icon
                        button.SetLockedState(false, true);
                    }
                    else
                    {
                        // Unlock (allows player to cycle notes)
                        button.Unlock();
                    }
                }
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
