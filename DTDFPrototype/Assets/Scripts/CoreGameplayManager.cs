using UnityEngine;

public class CoreGameplayManager : MonoBehaviour
{
    [Header("System References")]
    [SerializeField]
    internal TrayPositionManager trayAnimationManager;
    [SerializeField]
    internal HitTagEventRelay bellEventRelay;
    
    public GameState CurrentState { get; private set; }

    public AwaitingOrderState AwaitingOrderState { get; private set; }
    public ModifyOrderState ModifyOrderState { get; private set; }
    public ViewingResultState ViewingResultState { get; private set; }

    private void Awake()
    {
        AwaitingOrderState = new AwaitingOrderState(this);
        ModifyOrderState = new ModifyOrderState(this);
        ViewingResultState = new ViewingResultState(this);
        
        bellEventRelay.OnTagMatched.AddListener(SubmitOrder);
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
    
    public void SubmitOrder(RaycastHit hit)
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