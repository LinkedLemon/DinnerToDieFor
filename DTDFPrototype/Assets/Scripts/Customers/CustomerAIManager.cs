using System.Collections.Generic;
using UnityEngine;

public class CustomerAIManager : MonoBehaviour
{

    [SerializeField] private CustomerAI _customerPrefab;

    private Dictionary<CustomerRuntimeData, CustomerAI>
        _customerMap = new Dictionary<CustomerRuntimeData, CustomerAI>();

    [SerializeField] private Transform[] _seats = new Transform[5];
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _leavingSpot;

    public static CustomerAIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnCustomers(List<CustomerRuntimeData> customers)
    {
        // Cleanup existing if any (safety check)
        ClearAllCustomers();
        _customerMap.Clear();

        for (int i = 0; i < customers.Count; i++)
        {
            if (i >= _seats.Length) break;

            GameObject newCustomer =
                Instantiate(_customerPrefab.gameObject, _spawnPoint.position, _spawnPoint.rotation);
            CustomerAI ai = newCustomer.GetComponent<CustomerAI>();

            if (ai != null)
            {
                ai.WalkToSpot(_seats[i].position);
                ai.SetName(customers[i].Data.CustomerName);
                _customerMap.Add(customers[i], ai);
            }
        }
    }

    public void TriggerReaction(CustomerRuntimeData data, CustomerReaction reaction)
    {
        if (_customerMap.TryGetValue(data, out CustomerAI ai))
        {
            ai.DoReaction(reaction);
        }
    }

    public void CustomerLeaves(CustomerRuntimeData data)
    {
        if (_customerMap.TryGetValue(data, out CustomerAI ai))
        {
            ai.WalkToSpot(_leavingSpot.position);
            StartCoroutine(DestroyAfterDelay(ai.gameObject, 5f));
            _customerMap.Remove(data);
        }
    }

    public void CustomerDies(CustomerRuntimeData data)
    {
        if (_customerMap.TryGetValue(data, out CustomerAI ai))
        {
            ai.DoReaction(CustomerReaction.dead);
            ai.SetNameColor(Color.red);
            ai.HideModel();
        }
    }

    public void ClearAllCustomers()
    {
        foreach (var ai in _customerMap.Values)
        {
            if (ai != null)
            {
                ai.WalkToSpot(_leavingSpot.position);
                StartCoroutine(DestroyAfterDelay(ai.gameObject, 5f));
            }
        }

        _customerMap.Clear();
    }

    private System.Collections.IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) Destroy(obj);
    }
}
