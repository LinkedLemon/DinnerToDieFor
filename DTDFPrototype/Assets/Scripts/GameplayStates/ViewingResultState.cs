using UnityEngine;
using UnityEngine.InputSystem;

public class ViewingResultState : GameState
{
    private float startTime;
    private float duration = 5f;
    public ViewingResultState(CoreGameplayManager manager) : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering ViewingResultState");
        startTime = Time.time;

        ScoreResult result = ScoreManager.Instance.CalculateOrderScore();
        Debug.Log($"Score: {result.TotalScore} / {result.TargetScore}. Win: {result.Win}");

        if (result.Win)
        {
            OrderManager.Instance.ProcessNextOrder();
        }
        else
        {
            // For now, we'll just log a message and proceed to the next order anyway.
            Debug.Log("Order failed. Resetting for next order.");
            OrderManager.Instance.ProcessNextOrder();
        }
    }

    public override void Update()
    {
        if (Time.time - startTime > duration)
        {
            manager.TransitionToState(manager.AwaitingOrderState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting ViewingResultState");
        // TODO: Hide the result screen
        // Example: ResultScreen.Instance.Hide();
    }
}