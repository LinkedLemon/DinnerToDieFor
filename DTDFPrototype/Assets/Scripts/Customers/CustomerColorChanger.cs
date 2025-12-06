using UnityEngine;

public class CustomerColorChanger : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _maxRage;
    private Material _material;


    private void Start()
    {
        _material = _renderer.material;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateColor(float number)
    {
        _material.color = _gradient.Evaluate(number/_maxRage);
    }
}
