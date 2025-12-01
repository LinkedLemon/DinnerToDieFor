using UnityEngine;

public class ViewingResultState : GameState
{
    public ViewingResultState(GameplayManager manager) : base(manager) { }

    private float _autoExitTimer;

    public override void Enter()
    {
        Debug.Log("Entering ViewingResultState. Calculating Feedback...");
        
        // Calculate feedback immediately so icons/logs appear while viewing
        if (manager.roundManager != null)
        {
            manager.roundManager.CalculateAndShowFeedback();
        }

        Debug.Log("Will auto-exit in 5 seconds.");
        _autoExitTimer = 5.0f;
        
        // Enable camera movement UI (up/down arrows)
    }

    public override void Update()
    {
        // Handle viewing logic
        if (_autoExitTimer > 0)
        {
            _autoExitTimer -= Time.deltaTime;
            if (_autoExitTimer <= 0)
            {
                Debug.Log("Auto-exiting ViewingResultState.");
                OnDoneViewing();
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting ViewingResultState");
        // Disable camera movement UI
    }
    
    public void OnDoneViewing()
    {
        manager.FinishViewingResults();
    }
}
