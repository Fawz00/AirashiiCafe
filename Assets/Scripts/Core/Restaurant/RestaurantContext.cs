using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using UnityEngine.Events;

public class RestaurantContext : MonoBehaviour
{
    [Header("General Settings")]
    public float openDuration = 300f; // Duration the restaurant stays open in seconds

    [Header("Restaurant Context Components")]
    public PathFindNode entrance;
    public PathFindNode kitchen;
    public CustomerSpawner customerSpawner;
    public List<TableSet> tableSets = new List<TableSet>();
    public List<Maid> maids = new List<Maid>();

    [Header("Restaurant Menu Items")]
    public List<MenuItem_SO> menuItems = new List<MenuItem_SO>();

    [Header("Events")]
    public UnityEvent<int> onIncomeAdded;
    public UnityEvent onRestaurantClosed;

    public int incomeToday { get; private set; } = 0; // Total income for the current day
    public float remainingOpenTime { get; private set; } = 0f; // Remaining time the restaurant is open
    public int currentDay { get; private set; } = 0;

    private bool _dayFinishedPublished = false;

    private void Awake()
    {
        if (entrance == null)
        {
            Debug.LogError("RestaurantContext: Entrance is not assigned.");
        }
        if (kitchen == null)
        {
            Debug.LogError("RestaurantContext: Kitchen is not assigned.");
        }

        if (customerSpawner == null)
        {
            customerSpawner = FindFirstObjectByType<CustomerSpawner>();
            if (customerSpawner == null)
            {
                Debug.LogError("RestaurantContext: CustomerSpawner is not assigned and none found in the scene.");
            }
            else
            {
                Debug.Log("RestaurantContext: Found CustomerSpawner in the scene.");
            }
        }
        if (tableSets.Count == 0)
        {
            Debug.LogWarning("RestaurantContext: No TableSets assigned. Finding all TableSets in the scene.");

            TableSet[] foundTableSets = FindObjectsByType<TableSet>(FindObjectsSortMode.None);
            Debug.Log("Found " + foundTableSets.Length + " TableSets in the scene.");
            foreach (var tableSet in foundTableSets)
            {
                tableSets.Add(tableSet);
            }
        }
        if (maids.Count == 0)
        {
            Debug.LogWarning("RestaurantContext: No Maids assigned. Finding all Maids in the scene.");

            Maid[] foundMaids = FindObjectsByType<Maid>(FindObjectsSortMode.None);
            Debug.Log("Found " + foundMaids.Length + " Maids in the scene.");
            foreach (var maid in foundMaids)
            {
                maids.Add(maid);
            }
        }
    }

    private void Start()
    {
        currentDay = GameManager.Instance.playerData.day + 1;
        remainingOpenTime = openDuration;
        incomeToday = 0;

        UI_HUDRestaurant hudRestaurant = FindFirstObjectByType<UI_HUDRestaurant>();
        if (hudRestaurant != null)
        {
            hudRestaurant.SetRestaurantContext(this);
        }
        else
        {
            Debug.LogWarning("UI_HUDRestaurant not found in the scene. Please ensure it is present.");
        }
    }
    private void Update()
    {
        if (remainingOpenTime > 0)
        {
            remainingOpenTime -= Time.deltaTime;
            
            if (remainingOpenTime <= 0)
            {
                remainingOpenTime = 0;
                Debug.Log("RestaurantContext: Restaurant is now closed.");
                onRestaurantClosed?.Invoke();
            }
        }
        else
        {
            if (GetAllCustomers().Count == 0 && Time.time % 2 < 0.1f)
            {
                if (!_dayFinishedPublished)
                {
                    EventBus.Publish(new Event_OnDayFinished(incomeToday));
                    _dayFinishedPublished = true;
                }
            }
        }
    }
    void OnEnable()
    {
        EventBus.Subscribe<Event_OnDayFinished>(OnDayFinished);
    }
    void OnDisable()
    {
        EventBus.Unsubscribe<Event_OnDayFinished>(OnDayFinished);
    }

    private void OnDayFinished(Event_OnDayFinished finished)
    {
        Debug.Log($"RestaurantContext: Day finished with total income of {finished.incomeEarned}.");
        UIManager.Instance.AddUI(UIManager.Instance.GetUiFromResource("day_summary"), true, false, false);

        // Save to game data
        PlayerData_SO playerData = GameManager.Instance.playerData;
        playerData.coins += finished.incomeEarned;
        playerData.day = currentDay;
    }

    public TableSet GetAvailableTableSet()
    {
        foreach (var tableSet in tableSets)
        {
            if (tableSet.GetAvailableChairs().Count > 0)
            {
                return tableSet;
            }
        }
        return null;
    }
    public Chair GetAvailableChair()
    {
        foreach (var tableSet in tableSets)
        {
            if (tableSet.GetAvailableChairs().Count > 0)
            {
                return tableSet.GetAvailableChairs()[0];
            }
        }
        return null;
    }
    public List<Customer> GetAllCustomers()
    {
        List<Customer> customers = new List<Customer>();
        foreach (var tableSet in tableSets)
        {
            foreach (var chair in tableSet.chairs)
            {
                if (chair.isReserved && chair.reservedFor != null)
                {
                    customers.Add(chair.reservedFor);
                }
            }
        }
        return customers;
    }
    public List<Maid> GetAllAvailableMaid()
    {
        List<Maid> availableMaids = new List<Maid>();
        foreach (var maid in maids)
        {
            if (maid.isBusy == false)
            {
                availableMaids.Add(maid);
            }
        }
        return availableMaids;
    }
    public Maid GetAvailableMaid()
    {
        foreach (var maid in maids)
        {
            if (maid.isBusy == false)
            {
                return maid;
            }
        }
        return null;
    }

    public void AddIncome(int amount)
    {
        Debug.Log($"RestaurantContext: Adding income of {amount}. Total before adding: {incomeToday}");
        incomeToday += amount;
        onIncomeAdded?.Invoke(amount);
    }
}