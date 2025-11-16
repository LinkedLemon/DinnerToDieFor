using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderSO", menuName = "Scriptable Objects/OrderSO")]
public class OrderSO : ScriptableObject
{
    public List<FoodObject> _food;
}
