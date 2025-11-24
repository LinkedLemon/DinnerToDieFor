using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleCanvasOnInput : MonoBehaviour
{
    [Tooltip("The Canvas to show on awake and hide on input.")]
    public Canvas targetCanvas;

    private void Awake()
    {
        if (targetCanvas != null)
        {
            targetCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Target Canvas is not assigned on ToggleCanvasOnInput.", this);
        }
    }

    private void OnEnable()
    {
        // Ensure the singleton instance is available before subscribing
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack += HandleAttack;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks, especially when the object is destroyed
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAttack -= HandleAttack;
        }
    }

    private void HandleAttack(InputAction.CallbackContext context)
    {
        // This method is called when the OnAttack event is fired
        if (targetCanvas != null && targetCanvas.gameObject.activeSelf)
        {
            targetCanvas.gameObject.SetActive(false);
        }
    }
}
