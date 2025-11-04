using UnityEngine;
using UnityEngine.InputSystem;

public class ModifyOrderState : GameState
{
    public ModifyOrderState(CoreGameplayManager manager) : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering ModifyOrderState");
        // TODO: Unlock garnish and spy tool selection
        // Example: UIManager.Instance.EnableGarnishMenu();
    }

    public override void Update()
    {
        // This is a placeholder for the bell press event.
        if (Keyboard.current.lKey.isPressed)
        {
            manager.TransitionToState(manager.ViewingResultState);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting ModifyOrderState");
        // TODO: Lock garnish and spy tool selection
        // Example: UIManager.Instance.DisableGarnishMenu();
    }
}