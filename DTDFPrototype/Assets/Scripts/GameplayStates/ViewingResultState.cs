using UnityEngine;
using UnityEngine.InputSystem;

public class ViewingResultState : GameState
{
    public ViewingResultState(CoreGameplayManager manager) : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering ViewingResultState");
        // TODO: Display the outcome of the order
        // Example: ResultScreen.Instance.ShowResult(orderData);
    }

    public override void Update()
    {
        // This is a placeholder for returning to the start.
        if (Keyboard.current.jKey.isPressed)
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