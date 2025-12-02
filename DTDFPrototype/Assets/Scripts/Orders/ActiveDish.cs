/*
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A runtime class that represents a single dish currently in play.
/// It tracks the base dish data, its spawned GameObject, and the garnishes that have been added.
/// </summary>
public class ActiveDish
{
    public FoodObject DishSO { get; }
    public GameObject DishInstance { get; }
    public List<GarnishGameObject> AppliedGarnishes { get; }
    public int AddedGarnishCount => _addedGarnishCount;

    private const int MaxGarnishes = 3;
    private int _addedGarnishCount = 0;

    public ActiveDish(FoodObject dishSO, GameObject dishInstance)
    {
        DishSO = dishSO;
        DishInstance = dishInstance;
        AppliedGarnishes = new List<GarnishGameObject>();
    }

    /// <summary>
    /// Attempts to add a garnish to this dish.
    /// </summary>
    /// <param name="garnishSO">The garnish to add.</param>
    /// <returns>A result indicating success or the type of failure.</returns>
    public GarnishAddResult TryAddGarnish(GarnishGameObject garnishSO)
    {
        // First, check for duplicate garnishes. This is always a failure condition.
        if (AppliedGarnishes.Any(g => g._garnishType == garnishSO._garnishType))
        {
            Debug.LogWarning($"Duplicate garnish {garnishSO.name} attempted on {DishSO.name}.");
            return GarnishAddResult.Failure_DuplicateOrFull;
        }

        // Next, check if the absolute garnish limit has been reached.
        if (_addedGarnishCount >= MaxGarnishes)
        {
            Debug.LogWarning($"Garnish limit of {MaxGarnishes} reached for {DishSO.name}. Cannot add {garnishSO.name}.");
            return GarnishAddResult.Failure_DuplicateOrFull;
        }

        // The garnish is not a duplicate and there is space, so we will now process it.
        // Increment the total count regardless of whether it's good or bad.
        _addedGarnishCount++;

        if (DishSO._preferedGarnishes.Contains(garnishSO._garnishType))
        {
            AppliedGarnishes.Add(garnishSO);
            Debug.Log($"Added preferred garnish {garnishSO.name} to {DishSO.name}. Total added: {_addedGarnishCount}/{MaxGarnishes}");
            return GarnishAddResult.Success_Preferred;
        }

        // If it's not preferred, it's a "bad" garnish. It still counts towards the total.
        Debug.LogWarning($"Added bad garnish {garnishSO.name} to {DishSO.name}. Total added: {_addedGarnishCount}/{MaxGarnishes}");
        return GarnishAddResult.Failure_BadGarnish;
    }
}
*/
