using UnityEngine;

public class BriefcaseController : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private CoreGameplayManager coreGameplayManager;
    [SerializeField] private GameObject briefcaseVisual;
    
    private Animator _animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    public bool IsCaseOpen { get; private set; }

    private void Awake()
    {
        _animator = briefcaseVisual.GetComponent<Animator>();
        if (coreGameplayManager == null)
        {
            coreGameplayManager = FindFirstObjectByType<CoreGameplayManager>();
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnToggleBriefcase += ToggleBriefcase;
        }
        else
        {
            Debug.LogError("No Input Manager found");
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnToggleBriefcase -= ToggleBriefcase;
        }
    }

    private void ToggleBriefcase(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("CaseSwitched");
        
        if (coreGameplayManager.CurrentState == coreGameplayManager.ModifyOrderState && !_animator.IsInTransition(0))
        {
            IsCaseOpen = !IsCaseOpen;
            _animator.SetBool(IsOpen, IsCaseOpen);
        }
        else
        {
            Debug.Log("Input failed");
        }
    }
}
