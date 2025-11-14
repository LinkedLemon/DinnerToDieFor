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
    public bool TryAddGarnish(GarnishGameObject garnishSO)
    {
        if (AppliedGarnishes.Count >= MaxGarnishes)
        {
            Debug.LogWarning($"Garnish limit reached for {DishSO.name}. Cannot add {garnishSO.name}.");
            return false;
        }

        if (AppliedGarnishes.Any(g => g._garnishType == garnishSO._garnishType))
        {
            Debug.LogWarning($"Dish {DishSO.name} already has garnish of type {garnishSO._garnishType}. Cannot add another.");
            return false;
        }

        AppliedGarnishes.Add(garnishSO);
        Debug.Log($"Added {garnishSO.name} to {DishSO.name}. Total garnishes: {AppliedGarnishes.Count}");
        return true;
    }
}
