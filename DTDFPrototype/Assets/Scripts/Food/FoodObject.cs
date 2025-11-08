using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodObject", menuName = "Scriptable Objects/FoodObject")]
public class FoodObject : ScriptableObject
{
    public List<GarnishEnums.GarnishType> _preferedGarnishes;
    public List<GarnishEnums.GarnishType> _dislikedGarnishes;
}
