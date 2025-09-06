using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool isReserved { get; private set; } = false;
    public Customer reservedFor { get; private set; } = null;

    public bool isOccupied { get; private set; } = false;
    public Customer occupiedBy { get; private set; } = null;

    public void Reserve(Customer customer)
    {
        if (isReserved)
        {
            Debug.LogWarning("Chair is already reserved!");
            return;
        }

        isReserved = true;
        reservedFor = customer;
    }
    public void Sit(Customer customer)
    {
        if (isOccupied)
        {
            Debug.LogWarning("Chair is already occupied!");
            return;
        }
        if (!isReserved || reservedFor != customer)
        {
            Debug.LogWarning("Chair is not reserved for this customer!");
            return;
        }

        isOccupied = true;
        occupiedBy = customer;
    }
    public void StandUp(Customer customer)
    {
        if (!isOccupied || occupiedBy != customer)
        {
            Debug.LogWarning("Chair is not occupied by this customer!");
            return;
        }
        isReserved = false;
        reservedFor = null;

        isOccupied = false;
        occupiedBy = null;
    }

    // Draw gizmos to show chair direction in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 direction = transform.right; // Assuming the chair faces right
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}