using UnityEngine;

public class GameManagerCust : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject customerPrefab;
    public Transform[] spawnPoints;
    public Table[] tables;

    [Tooltip("Interval spawn dalam detik")]
    public float spawnInterval = 10f; // default 10 detik
    private float spawnTimer = 0f;

    [Header("Limit Customer (Opsional)")]
    public bool limitCustomer = true;   // kalau true, batasi jumlah customer
    public int maxCustomers = 2;        // batas maksimal customer aktif
    private int currentCustomers = 0;   // counter customer aktif

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
        // cek apakah batas customer aktif sudah tercapai
        if (limitCustomer && currentCustomers >= maxCustomers)
        {
            Debug.Log("Batas customer tercapai, tidak spawn baru.");
            return;
        }

        SpawnCustomer();
    }

    void SpawnCustomer()
    {
        // pilih spawn point random
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // buat customer di pintu
        GameObject newCustomer = Instantiate(customerPrefab, point.position, Quaternion.identity);

        // tambahkan counter
        currentCustomers++;

        // daftarkan ke script Customer supaya pas pergi counter berkurang
        Customer cust = newCustomer.GetComponent<Customer>();
        if (cust != null)
        {
            cust.OnCustomerLeave += HandleCustomerLeave;
        }

        // cari meja kosong
        foreach (Table table in tables)
        {
            if (table.HasEmptySeat())
            {
                cust.SitAtTable(table);
                return;
            }
        }

        // kalau semua penuh, customer pergi lagi
        Debug.Log("Semua meja penuh, customer tidak duduk.");
        Destroy(newCustomer, 1f);
        currentCustomers--;
    }

    void HandleCustomerLeave(Customer customer)
    {
        currentCustomers--;
    }
}

