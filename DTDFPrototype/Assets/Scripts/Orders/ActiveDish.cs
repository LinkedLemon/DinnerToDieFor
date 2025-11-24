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

    private const int MaxGarnishes = 3;

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
    /// <returns>True if the garnish was successfully added, false otherwise.</returns>
    public GarnishAddResult TryAddGarnish(GarnishGameObject garnishSO)
    {
        if (AppliedGarnishes.Count >= MaxGarnishes || AppliedGarnishes.Any(g => g._garnishType == garnishSO._garnishType))
        {
            Debug.LogWarning($"Garnish limit reached or duplicate garnish for {DishSO.name}. Cannot add {garnishSO.name}.");
            return GarnishAddResult.Failure_DuplicateOrFull;
        }

        if (DishSO._preferedGarnishes.Contains(garnishSO._garnishType))
        {
            AppliedGarnishes.Add(garnishSO);
            Debug.Log($"Added preferred garnish {garnishSO.name} to {DishSO.name}.");
            return GarnishAddResult.Success_Preferred;
        }

        // If it's not preferred, it's considered a "bad" garnish for this dish.
        // We don't add it to the list.
        Debug.LogWarning($"Added bad garnish {garnishSO.name} to {DishSO.name}.");
        return GarnishAddResult.Failure_BadGarnish;
    }
}
