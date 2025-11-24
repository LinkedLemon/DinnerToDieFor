using TMPro;
using UnityEngine;

public class CoreGameplayManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField]
    internal TrayPositionManager trayAnimationManager;
    [SerializeField]
    internal HitTagEventRelay bellEventRelay;
    
    [Header("UI References")]
    [SerializeField] private GameObject scoreScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Animation Settings")] 
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float scoreCountDuration = 1.0f;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float postAnimationDelay = 2.0f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header("Gameplay Settings")]
    [SerializeField] private float roundDuration = 120f;

    public GameState CurrentState { get; private set; }

    public AwaitingOrderState AwaitingOrderState { get; private set; }
    public ModifyOrderState ModifyOrderState { get; private set; }
    public ViewingResultState ViewingResultState { get; private set; }

    private bool _isSubmitting = false; // New flag to prevent re-submitting

    private void Awake()
    {
        AwaitingOrderState = new AwaitingOrderState(this);
        ModifyOrderState = new ModifyOrderState(this, roundDuration);
        ViewingResultState = new ViewingResultState(this, scoreScreen, winScreen, loseScreen, scoreText, slideDuration, scoreCountDuration, bounceDuration, postAnimationDelay, winSound, loseSound);
        
        bellEventRelay.OnMatch.AddListener(SubmitOrder);
    }

    private void Start()
    {
        TransitionToState(AwaitingOrderState);
        
        trayAnimationManager.OnReachedPointB.AddListener(() => TransitionToState(ModifyOrderState));
        trayAnimationManager.OnReachedPointC.AddListener(() => TransitionToState(ViewingResultState));
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
    
    public void SubmitOrder(string tag)
    {
        // Only allow submission if not already in the process of submitting
        if (_isSubmitting)
        {
            Debug.Log("Already submitting an order, ignoring bell ring.");
            return;
        }

        // Only allow submission from the ModifyOrderState
        if (CurrentState != ModifyOrderState)
        {
            Debug.LogWarning($"Bell rung in unexpected state: {CurrentState.GetType().Name}. Ignoring.");
            return;
        }

        _isSubmitting = true; // Set flag to prevent further submissions

        if (CurrentState is ISubmittableState submittableState)
        {
            submittableState.OnSubmit();
        }
        
        // This is protected by the CurrentState check above, but good for clarity
        if (CurrentState == ModifyOrderState) 
        {
            trayAnimationManager.SubmittedOrder();
        }
    }

    public void TransitionToState(GameState nextState)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        CurrentState = nextState;
        CurrentState.Enter();

        // Reset the submission flag when entering AwaitingOrderState
        // This signifies the end of one full submission cycle
        if (CurrentState == AwaitingOrderState)
        {
            _isSubmitting = false;
        }
    }
}