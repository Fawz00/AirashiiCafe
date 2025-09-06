using System.Collections.Generic;
using UnityEngine;

public class RestaurantContext : MonoBehaviour
{
    public PathFindNode entrance;
    public List<TableSet> tableSets = new List<TableSet>();

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
}