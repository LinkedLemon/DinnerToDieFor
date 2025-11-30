using System.Collections.Generic;
using UnityEngine;

public class CustomerAIManager : MonoBehaviour
{

    [SerializeField] private CustomerAI _customerPrefab;

    private List<OrderSO> _orders = new List<OrderSO>();
    private List<GameObject> _customers = new List<GameObject>();
    [SerializeField] private Transform[] _seats = new Transform[5];
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _leavingSpot;

    public static CustomerAIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject newCustomer = Instantiate(_customerPrefab.gameObject, _spawnPoint.position, _spawnPoint.rotation);
        newCustomer.GetComponent<CustomerAI>().WalkToSpot(_seats[0].position);
        newCustomer.GetComponent<CustomerAI>().SetName("Test");
    }

    public void ShowReactions(List<CustomerReaction> reactions)
    {
        for (int i = 0; i < _customers.Count; i++)
        {
            _customers[i].GetComponent<CustomerAI>().DoReaction(reactions[i]);
        }
    }

    public void StartRound(List<OrderSO> _orders)
    {
        for (int i =0; i < _customers.Count;i++)
        {
            Destroy(_customers[i]);
        }
        
        _customers.Clear();

        for (int i = 0; i<_orders.Count;i++)
        {
            GameObject newCustomer= Instantiate(_customerPrefab.gameObject,_spawnPoint.position,_spawnPoint.rotation);
            newCustomer.GetComponent<CustomerAI>().WalkToSpot(_seats[i].position);
            newCustomer.GetComponent<CustomerAI>().SetName("Test");
            _customers.Add(newCustomer);
        }
    }

    public void EndRound()
    {
        for (int i = 0; i<_customers.Count; i++)
        {
            _customers[i].GetComponent<CustomerAI>().WalkToSpot(_leavingSpot.position);
        }
    }
}
