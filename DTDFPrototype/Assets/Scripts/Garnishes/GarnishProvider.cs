using UnityEngine;

/// <summary>
/// Attach this component to any garnish prefab. It holds a reference to the 
/// GarnishGameObject ScriptableObject to provide its data to other systems.
/// </summary>
public class GarnishProvider : MonoBehaviour
{
    [Tooltip("The ScriptableObject representing this garnish.")]
    [SerializeField] private GarnishGameObject garnishData;

    public GarnishGameObject GarnishData => garnishData;
}
