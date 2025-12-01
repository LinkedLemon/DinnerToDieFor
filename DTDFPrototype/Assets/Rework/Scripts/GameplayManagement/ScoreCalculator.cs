using UnityEngine;

public enum CustomerReaction
{
    Positive,
    Meh,
    Negative,
    Dead
}

public struct DishCalculationResult
{
    public int FinalScore;
    public CustomerReaction Reaction;
    public bool ServedAllergy;
}

public static class ScoreCalculator
{
    public static DishCalculationResult CalculateDishScore(ActiveDish dish)
    {
        DishCalculationResult result = new DishCalculationResult();
        result.FinalScore = 0;
        result.ServedAllergy = false;

        CustomerRuntimeData customer = dish.CustomerData;

        foreach (var garnish in dish.AppliedGarnishes)
        {
            // Check for Allergy
            if (customer.CurrentAllergy == garnish)
            {
                result.ServedAllergy = true;
                result.Reaction = CustomerReaction.Dead;
                return result; // Immediate exit on allergy
            }

            // Calculate Score
            if (customer.CurrentGoodGarnishes.Contains(garnish))
            {
                result.FinalScore += 1;
            }
            else if (customer.CurrentBadGarnishes.Contains(garnish))
            {
                result.FinalScore -= 1;
            }
            // Meh garnishes add 0
        }

        // Determine Reaction based on Score
        if (result.FinalScore > 0)
        {
            result.Reaction = CustomerReaction.Positive;
        }
        else if (result.FinalScore < 0)
        {
            result.Reaction = CustomerReaction.Negative;
        }
        else
        {
            result.Reaction = CustomerReaction.Meh;
        }

        return result;
    }
}
