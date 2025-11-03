using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ObjectSpawner_InputSystem _spawner;
    void Start()
    {
        _spawner = FindFirstObjectByType<ObjectSpawner_InputSystem>();
        FindFirstObjectByType<MouseClickRaycast>().OnRaycastHit.AddListener(ClickedObject);
    }

    private void ClickedObject(RaycastHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Garnish"))
        {
            _spawner.SpawnObject();
        }
    }
}
