using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int roundCounter = 1;

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

        float targetScore = baseOrderValue + roundCounter;
        bool win = totalScore >= targetScore;

        if (win)
        {
            roundCounter++;
        }

        return new ScoreResult(totalScore, targetScore, win);
    }
}

public class ScoreResult
{
    public float TotalScore { get; }
    public float TargetScore { get; }
    public bool Win { get; }

    public ScoreResult(float totalScore, float targetScore, bool win)
    {
        TotalScore = totalScore;
        TargetScore = targetScore;
        Win = win;
    }
}
