using UnityEngine;

public class GameManagerCust : MonoBehaviour
{
    [Header("Spawn Settings (Wajib)")]
    public GameObject customerPrefab;
    public Transform[] spawnPoints;

    [Header("Table Settings (Wajib)")]
    public Table[] tables;

    [Header("Exit Settings")]
    public Transform exitPoint; // pintu keluar wajib

    [Tooltip("Interval spawn dalam detik")]
    public float spawnInterval = 10f;
    private float spawnTimer = 0f;

    [Header("Limit Customer (Opsional)")]
    public bool limitCustomer = true;
    public int maxCustomers = 2;
    private int currentCustomers = 0;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            TrySpawnCustomer();
            spawnTimer = 0f;
        }
    }

    void TrySpawnCustomer()
    {
        if (limitCustomer && currentCustomers >= maxCustomers)
        {
            Debug.Log("Batas customer tercapai, tidak spawn baru.");
            return;
        }

        SpawnCustomer();
    }

    void SpawnCustomer()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newCustomer = Instantiate(customerPrefab, point.position, Quaternion.identity);

        currentCustomers++;

        Customer cust = newCustomer.GetComponent<Customer>();
        if (cust != null)
        {
            cust.exitPoint = exitPoint; // wajib diisi

            cust.OnCustomerLeave += HandleCustomerLeave;

            foreach (Table table in tables)
            {
                if (table.HasEmptySeat())
                {
                    cust.SitAtTable(table);
                    return;
                }
            }
        }

        Debug.Log("Semua meja penuh, customer tidak duduk.");
        Destroy(newCustomer, 1f);
        currentCustomers--;
    }

    void HandleCustomerLeave(Customer customer)
    {
        currentCustomers--;
    }
}
