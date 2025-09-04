using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("Kursi di meja ini")]
    public Transform[] seatPoints;   // Drag Seat1 & Seat2
    private bool[] seatOccupied;

    void Awake()
    {
        seatOccupied = new bool[seatPoints.Length];
    }

    public bool HasEmptySeat()
    {
        foreach (bool seat in seatOccupied)
        {
            if (!seat) return true;
        }
        return false;
    }

    public int SeatCustomer(GameObject customer)
    {
        for (int i = 0; i < seatPoints.Length; i++)
        {
            if (!seatOccupied[i])
            {
                seatOccupied[i] = true;
                return i; // index kursi
            }
        }
        return -1;
    }

    public void ClearSeat(int index)
    {
        if (index >= 0 && index < seatOccupied.Length)
        {
            seatOccupied[index] = false;
        }
    }
}

