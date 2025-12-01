using UnityEngine;

[CreateAssetMenu(fileName = "GarnishGameObject", menuName = "Scriptable Objects/GarnishGameObject")]
public class GarnishGameObject : ScriptableObject
{
    public GarnishEnums.GarnishType _garnishType;
    public GameObject GarnishPrefab;
}
