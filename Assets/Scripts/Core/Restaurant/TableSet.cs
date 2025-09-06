using System.Collections.Generic;
using UnityEngine;

public class TableSet : MonoBehaviour
{
    [Header("Table Set Components")]
    public List<Chair> chairs = new List<Chair>();

    public List<Chair> GetAvailableChairs()
    {
        // Count the number of unoccupied chairs
        List<Chair> availableChairs = new List<Chair>();
        foreach (var chair in chairs)
        {
            if (!chair.isOccupied && !chair.isReserved)
            {
                availableChairs.Add(chair);
            }
        }
        return availableChairs;
    }
}