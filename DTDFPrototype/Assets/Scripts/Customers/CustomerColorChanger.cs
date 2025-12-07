using UnityEngine;

public class CustomerColorChanger : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _maxRage;
    private Material _material;
    private void Awake()
    {
        _material = _renderer.materials[0];
    }

    public void UpdateColor(float number)
    {
        _material.color = _gradient.Evaluate(number/_maxRage);
    }
}
