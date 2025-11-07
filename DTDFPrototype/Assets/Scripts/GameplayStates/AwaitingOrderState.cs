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

    }

    public override void Exit()
    {
        Debug.Log("Exiting AwaitingOrderState");
    }
}