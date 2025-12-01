using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerRuntimeData
{
    public CustomerScriptObject Data;
    public bool IsSpy;
    public int Patience;
    public bool IsAlive;
    public DishScriptObject AssignedDish;
    
    // Using a runtime duplicate of preferences to allow the Spy to shift tastes
    public List<GarnishEnums.GarnishType> CurrentGoodGarnishes;
    public List<GarnishEnums.GarnishType> CurrentMehGarnishes;
    public List<GarnishEnums.GarnishType> CurrentBadGarnishes;
    public GarnishEnums.GarnishType CurrentAllergy;

    public CustomerRuntimeData(CustomerScriptObject data, bool isSpy)
    {
        Data = data;
        IsSpy = isSpy;
        Patience = 10; // Starts at 10
        IsAlive = true;
        
        // Deep copy lists so we don't modify the SO asset
        CurrentGoodGarnishes = new List<GarnishEnums.GarnishType>(data.GoodGarnishes);
        CurrentMehGarnishes = new List<GarnishEnums.GarnishType>(data.MehGarnishes);
        CurrentBadGarnishes = new List<GarnishEnums.GarnishType>(data.BadGarnishes);
        CurrentAllergy = data.Allergy;
    }

    public void ModifyPatience(int amount)
    {
        Patience = Mathf.Clamp(Patience + amount, 0, 20);
        if (Patience <= 0)
        {
            IsAlive = false;
            Debug.Log($"{Data.CustomerName} has left due to lack of patience.");
        }
    }
}

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private List<string> _possibleNames;
    [SerializeField] private List<DishScriptObject> _availableDishes;
    [SerializeField] private CustomerScriptObject _templateCustomerSO; // Use this to clone or just as a type ref? 
    // Actually we need to generate new SOs or just use RuntimeData entirely?
    // The prompt says "create 5 randomly generated customer ScriptableObjects".
    // Runtime creation of SOs is possible but they don't persist in editor. That's fine.
    
    [Header("Runtime State")]
    public List<CustomerRuntimeData> ActiveCustomers = new List<CustomerRuntimeData>();
    public int RoundCount = 0;
    
    private List<ActiveDish> _currentRoundDishes = new List<ActiveDish>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartNewGame()
    {
        RoundCount = 0;
        GenerateNewCustomerSet();
        StartNewRound();
    }

    public void StartNewRound()
    {
        RoundCount++;
        AssignDishes();
        
        // Handle Spy Kill logic if applicable
        if (RoundCount > 1 && RoundCount % 2 != 0) 
        {
            if (RoundCount >= 3 && (RoundCount % 2 != 0)) 
            {
                SpyKill();
            }
        }
        
        if (CheckSpyWinCondition())
        {
            Debug.Log("Game Over: Spy won by killing everyone.");
            return; // TODO: Game Over State
        }

        if (RoundCount > 1)
        {
            ShiftSpyTastes();
        }
    }

    public void SpawnDishesOnTray(List<Transform> spawnPoints)
    {
        _currentRoundDishes.Clear();
        
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError($"[RoundManager] No dish spawn points provided to SpawnDishesOnTray! List is {(spawnPoints == null ? "null" : "empty")}. Check TrayPositionManager in Inspector.");
            return;
        }
        
        Debug.Log($"[RoundManager] Attempting to spawn dishes. Active Customers: {ActiveCustomers.Count}. Spawn Points: {spawnPoints.Count}");

        int pointIndex = 0;
        foreach (var customer in ActiveCustomers)
        {
            if (!customer.IsAlive) continue;

            if (pointIndex >= spawnPoints.Count)
            {
                Debug.LogWarning("[RoundManager] Not enough spawn points for all alive customers!");
                break;
            }

            if (customer.AssignedDish == null)
            {
                Debug.LogError($"[RoundManager] Customer {customer.Data.CustomerName} has no AssignedDish! Check '_availableDishes' in RoundManager.");
                continue;
            }
            
            if (customer.AssignedDish.DishPrefab == null)
            {
                Debug.LogError($"[RoundManager] AssignedDish '{customer.AssignedDish.name}' has no DishPrefab assigned!");
                continue;
            }

            Transform spawnPoint = spawnPoints[pointIndex];
            GameObject dishObj = Instantiate(customer.AssignedDish.DishPrefab, spawnPoint);
            dishObj.transform.localPosition = Vector3.zero;
            dishObj.transform.localRotation = Quaternion.identity;
            
            ActiveDish activeDish = new ActiveDish(customer.AssignedDish, customer, dishObj);
            
            DishTrigger trigger = dishObj.GetComponentInChildren<DishTrigger>();
            if (trigger != null)
            {
                trigger.Initialize(activeDish);
            }
            else
            {
                Debug.LogWarning($"Dish Prefab {dishObj.name} missing DishTrigger!");
            }
            
            _currentRoundDishes.Add(activeDish);
            Debug.Log($"[RoundManager] Spawned dish for {customer.Data.CustomerName} at Spawn Point {pointIndex}.");
            pointIndex++;
        }
    }

    private void GenerateNewCustomerSet()
    {
        ActiveCustomers.Clear();
        
        List<string> namePool = new List<string>(_possibleNames);
        if (namePool.Count < 5) Debug.LogWarning("Not enough unique names in pool!");

        int spyIndex = Random.Range(0, 5);

        for (int i = 0; i < 5; i++)
        {
            string cName = "Guest";
            if (namePool.Count > 0)
            {
                int nameIdx = Random.Range(0, namePool.Count);
                cName = namePool[nameIdx];
                namePool.RemoveAt(nameIdx);
            }

            CustomerScriptObject newSO = ScriptableObject.CreateInstance<CustomerScriptObject>();
            newSO.CustomerName = cName;
            newSO.name = cName; 

            GenerateRandomPreferences(newSO);

            bool isSpy = (i == spyIndex);
            CustomerRuntimeData customer = new CustomerRuntimeData(newSO, isSpy);
            ActiveCustomers.Add(customer);
        }
        
        Debug.Log($"Generated 5 customers. Spy is at index {spyIndex}.");
    }

    private void GenerateRandomPreferences(CustomerScriptObject so)
    {
        List<GarnishEnums.GarnishType> allTypes = new List<GarnishEnums.GarnishType>((GarnishEnums.GarnishType[])Enum.GetValues(typeof(GarnishEnums.GarnishType)));
        allTypes.Remove(GarnishEnums.GarnishType.none);

        // Shuffle
        for (int i = 0; i < allTypes.Count; i++)
        {
            GarnishEnums.GarnishType temp = allTypes[i];
            int randomIndex = Random.Range(i, allTypes.Count);
            allTypes[i] = allTypes[randomIndex];
            allTypes[randomIndex] = temp;
        }
        
        // Assign: 1 Allergy, others distributed
        so.Allergy = allTypes[0];
        
        // Determine splits. Prompt says "lists for good, meh, bad".
        // Let's say 1 Good, 2 Meh, rest Bad? Or random.
        // Prompt doesn't specify count, just "list for 3 garnishes... one for good, meh and bad". 
        // Wait, "list for 3 garnishes... one for good, meh and bad" might mean 3 lists? 
        // "The customer needs a list for 3 garnishes inside the garnishenum script, one for good, meh and bad."
        // This phrasing is slightly ambiguous. I will assume 3 lists: Good, Meh, Bad.
        // I'll distribute the remaining garnishes randomly.
        
        so.GoodGarnishes = new List<GarnishEnums.GarnishType>();
        so.MehGarnishes = new List<GarnishEnums.GarnishType>();
        so.BadGarnishes = new List<GarnishEnums.GarnishType>();

        // Assuming we have ~5 garnish types (Cherry, Olive, Paprika, Sword, Thyme).
        // 1 Allergy. 4 remaining.
        // 1 Good, 1 Meh, 2 Bad? Or random.
        
        // Let's pick randomly from remaining.
        for (int i = 1; i < allTypes.Count; i++)
        {
            int roll = Random.Range(0, 3); // 0: Good, 1: Meh, 2: Bad
            if (roll == 0) so.GoodGarnishes.Add(allTypes[i]);
            else if (roll == 1) so.MehGarnishes.Add(allTypes[i]);
            else so.BadGarnishes.Add(allTypes[i]);
        }
        
        // Ensure at least one good garnish?
        if (so.GoodGarnishes.Count == 0 && so.MehGarnishes.Count > 0)
        {
            so.GoodGarnishes.Add(so.MehGarnishes[0]);
            so.MehGarnishes.RemoveAt(0);
        }
    }

    private void AssignDishes()
    {
        if (_availableDishes == null || _availableDishes.Count == 0)
        {
            Debug.LogError("[RoundManager] No Available Dishes assigned in Inspector! Cannot assign dishes to customers.");
            return;
        }

        // Logic to assign dishes to alive customers
        // The prompt says "each customer will be assigned to a randomly picked dish that is available"
        // And "adding their customer SO to the dish trigger" (this part will be handled when spawning the dish visual).
        
        foreach (var customer in ActiveCustomers)
        {
            if (!customer.IsAlive) continue;
            
            customer.AssignedDish = _availableDishes[Random.Range(0, _availableDishes.Count)];
        }
        Debug.Log($"[RoundManager] Assigned dishes to {ActiveCustomers.Count} customers.");
    }

    private void SpyKill()
    {
        // "spy will kill 1 random normal customer"
        List<CustomerRuntimeData> potentialVictims = new List<CustomerRuntimeData>();
        foreach (var c in ActiveCustomers)
        {
            if (c.IsAlive && !c.IsSpy)
            {
                potentialVictims.Add(c);
            }
        }

        if (potentialVictims.Count > 0)
        {
            CustomerRuntimeData victim = potentialVictims[Random.Range(0, potentialVictims.Count)];
            victim.IsAlive = false;
            Debug.Log($"Spy killed {victim.Data.CustomerName}!");
            // TODO: Update visuals (empty chair)
        }
    }

    private bool CheckSpyWinCondition()
    {
        // "if after killing a customer only 1 other customer remains with them the game is lost"
        int aliveCount = 0;
        foreach (var c in ActiveCustomers)
        {
            if (c.IsAlive) aliveCount++;
        }
        
        // If Spy is alive and only 1 other person is alive (Total 2)
        // Assuming Spy is still alive here.
        CustomerRuntimeData spy = ActiveCustomers.Find(x => x.IsSpy);
        if (spy != null && spy.IsAlive && aliveCount <= 2)
        {
            return true; 
        }
        return false;
    }

    private void ShiftSpyTastes()
    {
        CustomerRuntimeData spy = ActiveCustomers.Find(x => x.IsSpy);
        if (spy == null || !spy.IsAlive) return;

        // "Spy will also pick one of their garnish preferences and swap it with another one"
        // We need to swap between Good/Meh/Bad lists.
        
        // Simple impl: Pick two random lists from (Good, Meh, Bad), pick one item from each, swap.
        List<List<GarnishEnums.GarnishType>> preferenceLists = new List<List<GarnishEnums.GarnishType>>
        {
            spy.CurrentGoodGarnishes,
            spy.CurrentMehGarnishes,
            spy.CurrentBadGarnishes
        };

        // Filter out empty lists if necessary, but we want to move stuff INTO empty lists too.
        
        int listIdxA = Random.Range(0, preferenceLists.Count);
        int listIdxB = Random.Range(0, preferenceLists.Count);
        
        while (listIdxA == listIdxB)
        {
            listIdxB = Random.Range(0, preferenceLists.Count);
        }

        List<GarnishEnums.GarnishType> listA = preferenceLists[listIdxA];
        List<GarnishEnums.GarnishType> listB = preferenceLists[listIdxB];

        if (listA.Count > 0)
        {
            GarnishEnums.GarnishType garnishToMove = listA[Random.Range(0, listA.Count)];
            listA.Remove(garnishToMove);
            listB.Add(garnishToMove);
            Debug.Log($"Spy changed taste: {garnishToMove} moved between preferences.");
        }
    }

    public void ProcessEndOfRound(CustomerRuntimeData servedCustomer, bool servedAllergy)
    {
       // Deprecated by CalculateRoundResults
    }
    
    private bool _roundEndedByAllergy = false;
    private bool _spyKilledByAllergy = false;

    public void CalculateAndShowFeedback()
    {
        _roundEndedByAllergy = false;
        _spyKilledByAllergy = false;

        foreach (var dish in _currentRoundDishes)
        {
            CustomerRuntimeData customer = dish.CustomerData;
            DishCalculationResult result = ScoreCalculator.CalculateDishScore(dish);

            if (result.ServedAllergy)
            {
                Debug.Log($"[RoundManager] Customer {customer.Data.CustomerName} served ALLERGY!");
                _roundEndedByAllergy = true;
                customer.IsAlive = false; 
                
                if (customer.IsSpy)
                {
                    _spyKilledByAllergy = true;
                }
            }
            else
            {
                int patienceChange = 0;
                switch (result.Reaction)
                {
                    case CustomerReaction.Positive:
                        patienceChange = 5;
                        break;
                    case CustomerReaction.Negative:
                        patienceChange = -5;
                        break;
                    case CustomerReaction.Meh:
                        patienceChange = 0;
                        break;
                }

                if (patienceChange != 0)
                {
                    customer.ModifyPatience(patienceChange);
                    Debug.Log($"Customer {customer.Data.CustomerName} reaction: {result.Reaction}. Patience change: {patienceChange}. Current: {customer.Patience}");
                }
                else
                {
                    Debug.Log($"Customer {customer.Data.CustomerName} reaction: {result.Reaction}. Patience unchanged.");
                }
            }
            // Do NOT destroy dishes here. Wait for viewing.
        }
    }

    public void CleanupAndStartNextRound()
    {
        // Cleanup visuals
        foreach (var dish in _currentRoundDishes)
        {
            if (dish.DishInstance != null)
            {
                Destroy(dish.DishInstance);
            }
        }
        _currentRoundDishes.Clear();

        // Check End Conditions
        if (_roundEndedByAllergy)
        {
            if (_spyKilledByAllergy)
            {
                Debug.Log("SPY KILLED BY ALLERGY! YOU WIN! Starting New Game.");
                StartNewGame(); 
            }
            else
            {
                Debug.Log("INNOCENT KILLED BY ALLERGY! YOU LOSE! Restarting.");
                StartNewGame(); 
            }
            return;
        }

        // Check Spy Win Condition (Normal Attrition)
        if (CheckSpyWinCondition())
        {
            Debug.Log("Game Over: Spy won by killing everyone.");
            StartNewGame(); // Or Game Over screen
            return;
        }

        // Proceed
        Debug.Log("Round Ended. Proceeding to next round.");
        StartNewRound();
    }

    // Deprecated single-call method, kept for interface compatibility if needed, 
    // but GameplayManager should call the new methods.
    public void CalculateRoundResults()
    {
        CalculateAndShowFeedback();
        CleanupAndStartNextRound();
    }
}
