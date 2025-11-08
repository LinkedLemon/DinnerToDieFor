using UnityEngine;

public class AdditiveScript : MonoBehaviour
{
    [SerializeField] private BaseAdditive _additive;

    public BaseAdditive GiveFoodObject()
    {
        return _additive;
    }
}
