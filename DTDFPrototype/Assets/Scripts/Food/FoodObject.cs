using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodObject", menuName = "Scriptable Objects/FoodObject")]
public class FoodObject : ScriptableObject
{
    public List<FoodEnums.GarnishType> _preferedGarnishes;
}
