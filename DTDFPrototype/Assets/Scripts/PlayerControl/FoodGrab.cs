using UnityEngine;

public class FoodGrab : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindFirstObjectByType<MouseClickRaycast>().OnRaycastHit.AddListener(ClickedObject);
    }

    private void ClickedObject(RaycastHit hit)
    {
        ObjectSpawner_InputSystem spawner = hit.collider.gameObject.GetComponent<ObjectSpawner_InputSystem>();
        if (spawner != null)
        {
            spawner.SpawnObject();
        }
    }
}
