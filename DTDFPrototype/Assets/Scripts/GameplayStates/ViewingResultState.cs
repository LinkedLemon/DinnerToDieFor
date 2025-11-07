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
        // TODO: Display the outcome of the order
        // Example: ResultScreen.Instance.ShowResult(orderData);
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