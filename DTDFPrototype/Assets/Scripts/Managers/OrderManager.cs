using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the queue of incoming orders, spawning dishes, and tracking their state.
/// </summary>
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Spawning")]
    [SerializeField]
    [Tooltip("The five locations where dishes will be spawned.")]
    private List<Transform> dishSpawnPoints;

    [Header("Order Queue")]
    [SerializeField]
    [Tooltip("For testing: Add Order ScriptableObjects here to populate the initial queue.")]
    private List<OrderSO> initialOrders;
    
    private Queue<OrderSO> _orderQueue = new Queue<OrderSO>();
    private OrderSO _currentOrder;
    private List<ActiveDish> _activeDishes = new List<ActiveDish>();

    public List<ActiveDish> ActiveDishes => _activeDishes;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Populate the queue with any initial orders set in the inspector
        foreach (var order in initialOrders)
        {
            AddOrderToQueue(order);
        }
    }

    /// <summary>
    /// Adds a new order to the processing queue.
    /// </summary>
    public void AddOrderToQueue(OrderSO order)
    {
        if (order != null)
        {
            _orderQueue.Enqueue(order);
        }
    }

    /// <summary>
    /// Processes the next order in the queue. Clears any existing dishes.
    /// </summary>
    [ContextMenu("Process Next Order")]
    public void ProcessNextOrder()
    {
        if (_orderQueue.Count == 0)
        {
            Debug.Log("No orders in the queue.");
            return;
        }

        ClearCurrentDishes();

        _currentOrder = _orderQueue.Dequeue();
        if (_currentOrder._food.Count > dishSpawnPoints.Count)
        {
            Debug.LogError($"Order '{_currentOrder.name}' has more dishes ({_currentOrder._food.Count}) than available spawn points ({dishSpawnPoints.Count}). Aborting.");
            return;
        }

        for (int i = 0; i < _currentOrder._food.Count; i++)
        {
            FoodObject dishSO = _currentOrder._food[i];
            Transform spawnPoint = dishSpawnPoints[i];

            if (dishSO.dishPrefab == null)
            {
                Debug.LogError($"Dish '{dishSO.name}' is missing its prefab!");
                continue;
            }

            // Spawn the dish prefab
            GameObject dishInstance = Instantiate(dishSO.dishPrefab, spawnPoint.position, spawnPoint.rotation);
            dishInstance.transform.SetParent(spawnPoint); // Optional: Keep the hierarchy clean

            // Create the runtime ActiveDish
            ActiveDish activeDish = new ActiveDish(dishSO, dishInstance);
            _activeDishes.Add(activeDish);

            // Find and initialize the trigger
            DishTrigger trigger = dishInstance.GetComponentInChildren<DishTrigger>();
            if (trigger != null)
            {
                trigger.Initialize(activeDish);
            }
            else
            {
                Debug.LogError($"The prefab for '{dishSO.name}' is missing the 'DishTrigger' component on a child object.", dishInstance);
            }
        }
    }

    /// <summary>
    /// Clears all currently active dishes from the scene.
    /// </summary>
    private void ClearCurrentDishes()
    {
        foreach (var activeDish in _activeDishes)
        {
            if (activeDish.DishInstance != null)
            {
                Destroy(activeDish.DishInstance);
            }
        }
        _activeDishes.Clear();
        _currentOrder = null;
    }
}
