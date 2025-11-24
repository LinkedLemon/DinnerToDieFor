using UnityEngine;

public class ModifyOrderState : GameState, ISubmittableState
{
    private readonly float _roundDuration;
    private float _startTime;
    private bool _timerRunning;

    public ModifyOrderState(CoreGameplayManager manager, float roundDuration) : base(manager)
    {
        _roundDuration = roundDuration;
    }

    public override void Enter()
    {
        Debug.Log("Entering ModifyOrderState");
        _startTime = Time.time;
        _timerRunning = true;
        // TODO: Unlock garnish and spy tool selection
        // Example: UIManager.Instance.EnableGarnishMenu();
    }

    public override void Update()
    {
        if (_timerRunning)
        {
            float elapsedTime = Time.time - _startTime;
            // TODO: Update UI with remaining time: (_roundDuration - elapsedTime)
            
            if (elapsedTime >= _roundDuration)
            {
                // Note: The automatic submission will call OnSubmit via the CoreGameplayManager
                Debug.Log("Time is up! Submitting order automatically.");
                manager.SubmitOrder(null);
            }
        }
    }

    public override void Exit()
    {
        _timerRunning = false;
        Debug.Log("Exiting ModifyOrderState");
        // TODO: Lock garnish and spy tool selection
        // Example: UIManager.Instance.DisableGarnishMenu();
    }

    public void OnSubmit()
    {
        float remainingTime = 0;
        if (_timerRunning)
        {
            // This block runs on manual submission before time runs out
            float elapsedTime = Time.time - _startTime;
            remainingTime = Mathf.Max(0, _roundDuration - elapsedTime);
            Debug.Log($"Order submitted manually with {remainingTime:F2}s left.");
        }
        else
        {
            // This block runs if OnSubmit is called after the timer has already run out
            Debug.Log("Order submitted automatically. No time remaining.");
        }

        // Pass timing data to the ScoreManager for it to calculate the multiplier
        ScoreManager.Instance.TimeRemainingOnSubmission = remainingTime;
        ScoreManager.Instance.RoundDurationOnSubmission = _roundDuration;
        
        _timerRunning = false; // Ensure timer stops on submission
    }
}