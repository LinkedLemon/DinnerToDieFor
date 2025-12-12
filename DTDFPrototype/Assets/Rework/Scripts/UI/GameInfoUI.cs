using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem; // Added for InputAction.CallbackContext

public class GameInfoUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _startPopup;
    [SerializeField] private GameObject _spyKillPopup;
    
    private Coroutine _startPopupCoroutine;
    private Coroutine _spyKillPopupCoroutine;

    private void Awake()
    {
        if (_startPopup != null) _startPopup.SetActive(false);
        if (_spyKillPopup != null) _spyKillPopup.SetActive(false);
    }

    public void ShowStartPopup(bool autoHide)
    {
        if (_startPopup == null) return;

        _startPopup.SetActive(true);
        
        if (_startPopupCoroutine != null) StopCoroutine(_startPopupCoroutine);

        if (autoHide)
        {
            _startPopupCoroutine = StartCoroutine(HidePopupRoutine(_startPopup, 2.0f));
        }
        else
        {
            // Subscribe to mouse click if not auto-hiding
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnAttack += HandleLeftClickToHideStartPopup;
            }
        }
    }

    public void HideStartPopup()
    {
        if (_startPopup != null) _startPopup.SetActive(false);
        // Unsubscribe from mouse click event
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= HandleLeftClickToHideStartPopup;
        }
    }

    private void HandleLeftClickToHideStartPopup(InputAction.CallbackContext context)
    {
        HideStartPopup();
    }

    public void ShowSpyKillPopup()
    {
        if (_spyKillPopup == null) return;

        _spyKillPopup.SetActive(true);
        
        if (_spyKillPopupCoroutine != null) StopCoroutine(_spyKillPopupCoroutine);
        _spyKillPopupCoroutine = StartCoroutine(HidePopupRoutine(_spyKillPopup, 2.0f));
    }

    private IEnumerator HidePopupRoutine(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popup != null) popup.SetActive(false);
        // Unsubscribe if this was the start popup, just in case
        if (popup == _startPopup && !popup.activeSelf) 
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnAttack -= HandleLeftClickToHideStartPopup;
            }
        }
    }
}
