using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Managers")]
    public TrayPositionManager trayManager;
    public RoundManager roundManager;
    public CameraControlManager cameraControl;

    [Header("Audio")]
    [SerializeField] private AudioClip bellSound;

    [Header("Win/Lose UI")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

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
            roundManager.OnNewRoundStarted.AddListener(() => 
            {
                HideEndScreens();
                TransitionToState(AwaitingOrderState);
            });
            
            roundManager.OnGameWon.AddListener(ShowWinScreen);
            roundManager.OnGameLost.AddListener(ShowLoseScreen);

            roundManager.StartNewGame();
        }
        // Removed manual TransitionToState here to wait for RoundManager
        
        SoundManager.instance.PlayMusic(MusicType.Game, 0.3f);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    private void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
        if (SoundManager.instance != null && winSound != null) SoundManager.instance.PlayAudioClip(winSound, 1);
    }

    private void ShowLoseScreen()
    {
        if (loseScreen != null) loseScreen.SetActive(true);
        if (SoundManager.instance != null && loseSound != null) SoundManager.instance.PlayAudioClip(loseSound, 1);
    }

    private void HideEndScreens()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
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
        // Only allow submission if we are in ModifyOrderState
        if (_currentState != ModifyOrderState) return;

        // Logic when player submits dish
        trayManager.SubmittedOrder();
        if (cameraControl != null) cameraControl.TriggerLookUp();
        if (SoundManager.instance != null && bellSound != null) SoundManager.instance.PlayAudioClip(bellSound, 1);
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
    }

    public void TrySubmitCurrentState()
    {
        if (_currentState is ISubmittableState submittableState)
        {
            submittableState.OnSubmit();
        }
    }
}
