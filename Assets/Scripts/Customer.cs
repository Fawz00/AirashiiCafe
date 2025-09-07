using UnityEngine;
using System;

public class Customer : MonoBehaviour
{
    private Table currentTable;
    private int seatIndex = -1;

    [Header("Durasi & Kecepatan")]
    public float stayDuration = 5f;
    public float moveSpeed = 2f;

    private float stayTimer;
    private int currentPathIndex = 0;
    private bool isMoving = false;
    private bool isSeated = false;
    private bool isLeaving = false;

    [Header("Path Settings (Opsional)")]
    public Transform[] entryPath;   // opsional → kalau ada, jalur ke kursi
    public Transform[] exitPath;    // opsional → kalau ada, jalur ke pintu
    [HideInInspector] public Transform exitPoint; // wajib → fallback pintu keluar

    public event Action<Customer> OnCustomerLeave;

    void Update()
    {
        if (isMoving)
        {
            if (!isLeaving) // jalur masuk
            {
                if (entryPath != null && entryPath.Length > 0)
                    MoveAlongPath(entryPath, afterSeat: true);
                else
                    MoveToDirect(currentTable.seatPoints[seatIndex].position, afterSeat: true);
            }
            else // jalur keluar
            {
                if (exitPath != null && exitPath.Length > 0)
                    MoveAlongPath(exitPath, afterSeat: false);
                else if (exitPoint != null)
                    MoveToDirect(exitPoint.position, afterSeat: false);
                else
                {
                    Debug.LogWarning("ExitPoint kosong! Customer dihapus langsung.");
                    OnCustomerLeave?.Invoke(this);
                    Destroy(gameObject);
                }
            }
        }

        if (isSeated)
        {
            Debug.Log("Customer sedang duduk di meja.");
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayDuration)

                LeaveTable();
                Debug.Log("Customer selesai makan dan akan pergi.");
        }
    }

    private void MoveAlongPath(Transform[] path, bool afterSeat)
    {
        if (currentPathIndex >= path.Length) return;

        Transform targetPoint = path[currentPathIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentPathIndex++;
            if (currentPathIndex >= path.Length)
            {
                isMoving = false;

                if (afterSeat)
                {
                    isSeated = true;
                    stayTimer = 0f;
                }
                else
                {
                    OnCustomerLeave?.Invoke(this);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void MoveToDirect(Vector3 target, bool afterSeat)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            isMoving = false;

            if (afterSeat)
            {
                isSeated = true;
                stayTimer = 0f;
            }
            else
            {
                OnCustomerLeave?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    public void SitAtTable(Table table)
    {
        if (isSeated || isMoving || table == null) return;

        int seat = table.SeatCustomer(gameObject);
        if (seat == -1) return;

        currentTable = table;
        seatIndex = seat;

        currentPathIndex = 0;
        isMoving = true;
        isLeaving = false;
    }

    public void LeaveTable()
    {
        if (currentTable != null && seatIndex != -1)
            currentTable.ClearSeat(seatIndex);

        currentTable = null;
        seatIndex = -1;
        isSeated = false;

        currentPathIndex = 0;
        isMoving = true;
        isLeaving = true;
    }
}
