using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class AwaitingOrderState : GameState
{
    public AwaitingOrderState(CoreGameplayManager manager) : base(manager) { }
    
    public override void Enter()
    {
        Debug.Log("Entering AwaitingOrderState");
        
        bool hasOrder = OrderManager.Instance.ProcessNextOrder();

        if (hasOrder)
        {
            manager.trayAnimationManager.SendOrder();
        }
        else
        {
            // Optional: Transition to a 'Game Over' state or display a message
            Debug.Log("All orders completed!");
            // For now, the game will idle here. A transition to a game over/credits screen could be added.
        }
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exiting AwaitingOrderState");
    }
}