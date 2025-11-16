using UnityEngine;

[CreateAssetMenu(fileName = "BaseAdditive", menuName = "Scriptable Objects/BaseAdditive")]
public class BaseAdditive : ScriptableObject
{
    public string _name;

    public float _pointValue;

    public float _failedPointValue;

    public float _heatValue;

    public float _failedHeatValue;


}
