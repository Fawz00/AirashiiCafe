using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private RestaurantContext restaurantContext;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] [Range(0, 10)] private int randomness = 9;

    private float timer = 0f;

    private void Start()
    {
        if (restaurantContext == null)
        {
            restaurantContext = FindFirstObjectByType<RestaurantContext>();
        }
    }
    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            int randomValue = Random.Range(0, 10);
            if (randomValue < randomness)
            {
                SpawnCustomer();
            }
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null || restaurantContext == null || restaurantContext.entrance == null)
        {
            Debug.LogWarning("CustomerSpawner: Missing references.");
            return;
        }

        Instantiate(customerPrefab, restaurantContext.entrance.transform.position, Quaternion.identity);
    }
}