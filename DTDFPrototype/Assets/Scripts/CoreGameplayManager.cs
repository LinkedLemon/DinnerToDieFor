using UnityEngine;

public class CoreGameplayManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField]
    internal TrayPositionManager trayAnimationManager;
    
    private GameState currentState;

    public AwaitingOrderState AwaitingOrderState { get; private set; }
    public ModifyOrderState ModifyOrderState { get; private set; }
    public ViewingResultState ViewingResultState { get; private set; }

    private void Awake()
    {
        AwaitingOrderState = new AwaitingOrderState(this);
        ModifyOrderState = new ModifyOrderState(this);
        ViewingResultState = new ViewingResultState(this);
    }

    private void Start()
    {
        // Initial state
        TransitionToState(AwaitingOrderState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void TransitionToState(GameState nextState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = nextState;
        currentState.Enter();
    }
}