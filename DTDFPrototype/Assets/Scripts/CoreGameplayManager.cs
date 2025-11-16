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

    public GameState CurrentState { get; private set; }

    public AwaitingOrderState AwaitingOrderState { get; private set; }
    public ModifyOrderState ModifyOrderState { get; private set; }
    public ViewingResultState ViewingResultState { get; private set; }

    private void Awake()
    {
        AwaitingOrderState = new AwaitingOrderState(this);
        ModifyOrderState = new ModifyOrderState(this);
        ViewingResultState = new ViewingResultState(this, scoreScreen, winScreen, loseScreen, scoreText, slideDuration, scoreCountDuration, bounceDuration, postAnimationDelay);
        
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
    }
}