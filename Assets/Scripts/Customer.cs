using UnityEngine;
using System;

public class Customer : MonoBehaviour
{
    public bool isSeated = false;
    private Table currentTable;
    private int seatIndex = -1;

    [Header("Customer Timer")]
    public float stayDuration = 5f;  
    private float stayTimer = 0f;

    [Header("Movement")]
    public float moveSpeed = 2f;        
    private Vector3 targetPosition;     
    private bool isMoving = false;      

    private Animator anim;

    // Event: kasih tahu GameManager kalau customer sudah pergi
    public event Action<Customer> OnCustomerLeave;

    void Start()
    {
        anim = GetComponent<Animator>();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (anim != null) anim.SetBool("isWalking", true);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
                isSeated = true;
                stayTimer = 0f;
                if (anim != null) anim.SetBool("isWalking", false);
            }
        }

        if (isSeated)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayDuration)
            {
                LeaveTable();
            }
        }
    }

    public void SitAtTable(Table table)
    {
        if (isSeated || isMoving) return;

        int seat = table.SeatCustomer(this.gameObject);
        if (seat != -1)
        {
            currentTable = table;
            seatIndex = seat;

            targetPosition = table.seatPoints[seat].position;
            isMoving = true;
        }
    }

    public void LeaveTable()
    {
        if (currentTable != null && seatIndex != -1)
        {
            currentTable.ClearSeat(seatIndex);
        }

        isSeated = false;
        seatIndex = -1;
        currentTable = null;

        // kasih tahu GameManager
        OnCustomerLeave?.Invoke(this);

        Destroy(this.gameObject);
    }
}
