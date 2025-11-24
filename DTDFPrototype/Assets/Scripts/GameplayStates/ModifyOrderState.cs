using UnityEngine;
using UnityEngine.UI;

public class ModifyOrderState : GameState, ISubmittableState
{
    private readonly float _roundDuration;
    private readonly Image _timerBar;
    private readonly Image _colorBar;
    private readonly Gradient _timerGradient;
    
    private float _startTime;
    private bool _timerRunning;

    public ModifyOrderState(CoreGameplayManager manager, float roundDuration, Image timerBar, Image colorBar, Gradient timerGradient) : base(manager)
    {
        _roundDuration = roundDuration;
        _timerBar = timerBar;
        _colorBar =  colorBar;
        _timerGradient = timerGradient;
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
            float remainingTime = _roundDuration - elapsedTime;
            
            // Update UI
            if (_timerBar != null)
            {
                float normalizedTime = Mathf.Clamp01(remainingTime / _roundDuration);
                _timerBar.fillAmount = normalizedTime;

                if (_timerGradient != null)
                {
                    _colorBar.color = _timerGradient.Evaluate(1 - normalizedTime); // Evaluate from 0 (green) to 1 (red)
                }
            }
            
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