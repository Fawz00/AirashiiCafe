using System.Collections.Generic;
using UnityEngine;

public class RestaurantContext : MonoBehaviour
{
    [Header("Restaurant Context Components")]
    public PathFindNode entrance;
    public PathFindNode kitchen;
    public List<TableSet> tableSets = new List<TableSet>();
    public List<Maid> maids = new List<Maid>();

    [Header("Restaurant Menu Items")]
    public List<BaseItem_SO> menuItems = new List<BaseItem_SO>();

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
}