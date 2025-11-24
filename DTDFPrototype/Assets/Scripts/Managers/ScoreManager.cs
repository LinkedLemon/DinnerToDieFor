using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Scoring Settings")]
    [SerializeField] private float maxBonusMultiplier = 0.5f;
    [SerializeField] private float targetPointsPerRound = 25f;

    public int roundCounter = 1;

    // Properties to hold timing data from the gameplay state
    public float TimeRemainingOnSubmission { get; set; }
    public float RoundDurationOnSubmission { get; set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public float GetCurrentTargetScore()
    {
        List<ActiveDish> activeDishes = OrderManager.Instance.ActiveDishes;
        float baseOrderValue = 0.0f;

        foreach (var activeDish in activeDishes)
        {
            baseOrderValue += 100.0f; // Assuming a base value for each dish
        }

        float baseScoreWithInstantBonus = baseOrderValue * (1 + maxBonusMultiplier);
        float targetScore = baseScoreWithInstantBonus + (roundCounter * targetPointsPerRound);
        return targetScore;
    }

    public ScoreResult CalculateOrderScore()
    {
        List<ActiveDish> activeDishes = OrderManager.Instance.ActiveDishes;
        float totalScore = 0.0f;
        float baseOrderValue = 0.0f;

        foreach (var activeDish in activeDishes)
        {
            float dishScore = 100.0f;
            baseOrderValue += 100.0f;

            List<GarnishGameObject> appliedGarnishes = activeDish.AppliedGarnishes;
            FoodObject dishSO = activeDish.DishSO;

            foreach (var appliedGarnish in appliedGarnishes)
            {
                if (dishSO._preferedGarnishes.Contains(appliedGarnish._garnishType))
                {
                    dishScore += appliedGarnish._pointValue;
                }
                else if (dishSO._dislikedGarnishes.Contains(appliedGarnish._garnishType))
                {
                    dishScore -= appliedGarnish._failedPointValue;
                }
            }
            totalScore += dishScore;
        }

        // Apply the time bonus as a multiplier
        float timeMultiplier = 1.0f;
        if (RoundDurationOnSubmission > 0) // Avoid division by zero
        {
            timeMultiplier = 1.2f + (TimeRemainingOnSubmission / RoundDurationOnSubmission) * maxBonusMultiplier;
            Debug.Log($"Time Multiplier: {timeMultiplier:F2}x");
        }
        totalScore *= timeMultiplier;

        // Reset the submission time fields for the next round
        TimeRemainingOnSubmission = 0;
        RoundDurationOnSubmission = 0;

        // --- New Target Score Logic ---
        float baseScoreWithInstantBonus = baseOrderValue * (1 + maxBonusMultiplier);
        float targetScore = baseScoreWithInstantBonus + (roundCounter * targetPointsPerRound);
        
        int finalScore = Mathf.RoundToInt(totalScore);

        bool win = finalScore >= targetScore;

        if (win)
        {
            roundCounter++;
        }

        return new ScoreResult(finalScore, targetScore, win);
    }
}

public class ScoreResult
{
    public int TotalScore { get; }
    public float TargetScore { get; }
    public bool Win { get; }

    public ScoreResult(int totalScore, float targetScore, bool win)
    {
        TotalScore = totalScore;
        TargetScore = targetScore;
        Win = win;
    }
}

