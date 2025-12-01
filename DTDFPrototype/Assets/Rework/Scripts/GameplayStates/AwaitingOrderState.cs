using UnityEngine;

public class AwaitingOrderState : GameState
{
    public AwaitingOrderState(GameplayManager manager) : base(manager) { }
    
    public override void Enter()
    {
        Debug.Log("Entering AwaitingOrderState");
        
        // TODO: Check if customers are seated and ready.
        // For now, immediately proceed to simulate flow.
        // In the future, this will listen for an event or check customer status.
        
        StartOrderProcess();
    }

    private void StartOrderProcess()
    {
        Debug.Log("Customers ready. Sending tray.");
        manager.StartNextOrder();
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exiting AwaitingOrderState");
    }
}
