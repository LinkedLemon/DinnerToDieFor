using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reworked ActiveDish that validates garnishes against a Customer's preferences
/// rather than the Dish's fixed preferences.
/// </summary>
public class ActiveDish
{
    public DishScriptObject DishData { get; }
    public CustomerRuntimeData CustomerData { get; }
    public GameObject DishInstance { get; }
    
    public List<GarnishEnums.GarnishType> AppliedGarnishes { get; }
    
    private const int MaxGarnishes = 3;

    public ActiveDish(DishScriptObject dishData, CustomerRuntimeData customerData, GameObject dishInstance)
    {
        DishData = dishData;
        CustomerData = customerData;
        DishInstance = dishInstance;
        AppliedGarnishes = new List<GarnishEnums.GarnishType>();
    }

    public GarnishAddResult TryAddGarnish(GarnishEnums.GarnishType garnishType)
    {
        // Check for Duplicate
        if (AppliedGarnishes.Contains(garnishType))
        {
            return GarnishAddResult.Failure_DuplicateOrFull;
        }
        
        // Check for Full
        if (AppliedGarnishes.Count >= MaxGarnishes)
        {
            return GarnishAddResult.Failure_DuplicateOrFull;
        }

        AppliedGarnishes.Add(garnishType);

        // Check Logic against Customer Preferences
        if (CustomerData.CurrentGoodGarnishes.Contains(garnishType))
        {
            return GarnishAddResult.Success_Preferred;
        }
        else if (CustomerData.CurrentMehGarnishes.Contains(garnishType))
        {
            // Using Failure_BadGarnish or create a new result for Meh?
            // The prompt asked for "good, meh and bad"
            // The Enum GarnishAddResult needs to support Meh.
            // The old enum had "Failure_Duplicate", "Failure_BadGarnish".
            // I should probably update GarnishEnums.cs to have explicit results.
            return GarnishAddResult.Success_Meh; 
        }
        else if (CustomerData.CurrentBadGarnishes.Contains(garnishType))
        {
             return GarnishAddResult.Failure_BadGarnish;
        }
        else if (CustomerData.CurrentAllergy == garnishType)
        {
            // It's an allergy! Technically "Bad" but fatal.
            // For visual feedback, maybe just "Bad" for now? 
            // Or we can return a specific Allergy result if we want immediate feedback (which we probably don't want to reveal immediately?)
            // The prompt says: "tell of the spy is him changing his taste...".
            // "if they get a bad dish they begin to loose patience".
            // "If someone has died from an allergy...".
            return GarnishAddResult.Failure_Allergy;
        }
        
        // Fallback
        return GarnishAddResult.Failure_BadGarnish;
    }
}
