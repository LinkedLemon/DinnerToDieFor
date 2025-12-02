using UnityEngine;

public class ModifyOrderState : GameState, ISubmittableState
{
    public ModifyOrderState(GameplayManager manager) : base(manager)
    {
    }

    public override void Enter()
    {
        Debug.Log("Entering ModifyOrderState");
        // Enable interaction with garnishes
        // Show UI if needed
    }

    public override void Update()
    {
        // Check for input?
        // Wait for player to press Submit button
    }

    public override void Exit()
    {
        Debug.Log("Exiting ModifyOrderState");
        // Disable interaction
    }

    public void OnSubmit()
    {
        Debug.Log("Order Submitted!");
        manager.SubmitOrder();
    }
}
