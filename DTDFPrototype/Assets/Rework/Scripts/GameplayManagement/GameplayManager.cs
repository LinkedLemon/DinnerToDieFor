using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Managers")]
    public TrayPositionManager trayManager;
    public RoundManager roundManager;

    [Header("Events")]
    public UnityEvent OnOrderSubmitted;
    public UnityEvent OnRoundEnded;

    // State Machine
    private GameState _currentState;
    
    public AwaitingOrderState AwaitingOrderState { get; private set; }
    public ModifyOrderState ModifyOrderState { get; private set; }
    public ViewingResultState ViewingResultState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeStates();
    }

    private void Start()
    {
        if (roundManager != null)
        {
            roundManager.StartNewGame();
        }
        TransitionToState(AwaitingOrderState);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    private void InitializeStates()
    {
        AwaitingOrderState = new AwaitingOrderState(this);
        ModifyOrderState = new ModifyOrderState(this);
        ViewingResultState = new ViewingResultState(this);
    }

    public void TransitionToState(GameState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentState.Enter();
    }

    // Bridge methods for states to call
    public void StartNextOrder()
    {
        // Logic to setup next order
        if (roundManager != null && trayManager != null)
        {
            roundManager.SpawnDishesOnTray(trayManager.DishSpawnPoints);
        }
        trayManager.SendOrder();
        TransitionToState(ModifyOrderState);
    }

    public void SubmitOrder()
    {
        // Logic when player submits dish
        trayManager.SubmittedOrder();
        OnOrderSubmitted?.Invoke();
        TransitionToState(ViewingResultState);
    }

    public void FinishViewingResults()
    {
        // Logic when done viewing results
        if (roundManager != null)
        {
            roundManager.CleanupAndStartNextRound();
        }
        OnRoundEnded?.Invoke();
        TransitionToState(AwaitingOrderState);
    }

    public void TrySubmitCurrentState()
    {
        if (_currentState is ISubmittableState submittableState)
        {
            submittableState.OnSubmit();
        }
    }
}
