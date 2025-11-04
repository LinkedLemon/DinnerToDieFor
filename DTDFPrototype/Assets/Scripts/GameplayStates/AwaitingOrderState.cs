using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class AwaitingOrderState : GameState
{
    public AwaitingOrderState(CoreGameplayManager manager) : base(manager) { }
    
    public override void Enter()
    {
        Debug.Log("Entering AwaitingOrderState");

        manager.trayAnimationManager.SendOrder();
    }

    public override void Update()
    {
        // This is a placeholder for the tray arrival event.
        // In a real implementation, an event from a tray manager would trigger the transition.
        if (Keyboard.current.spaceKey.isPressed) 
        {
            manager.TransitionToState(manager.ModifyOrderState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting AwaitingOrderState");
    }
}