using UnityEngine;

public class ViewingResultState : GameState
{
    public ViewingResultState(GameplayManager manager) : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering ViewingResultState. Calculating Feedback...");
        
        if (manager.roundManager != null)
        {
            // Start the sequential feedback routine
            manager.StartCoroutine(manager.roundManager.CalculateAndShowFeedbackRoutine(() => 
            {
                Debug.Log("Feedback sequence finished.");
                OnDoneViewing();
            }));
        }
        else
        {
            OnDoneViewing();
        }
    }

    public override void Update()
    {
        // Waiting for coroutine to finish
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
