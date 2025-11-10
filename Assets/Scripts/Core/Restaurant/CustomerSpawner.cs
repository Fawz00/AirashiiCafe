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

        if (restaurantContext != null)
        {
            restaurantContext.onRestaurantClosed.AddListener(OnRestaurantClosed);
        }
        else
        {
            Debug.LogError("CustomerSpawner: RestaurantContext is not assigned and none found in the scene.");
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval && restaurantContext != null && restaurantContext.entrance != null)
        {
            timer = 0f;
            int randomValue = Random.Range(0, 10);
            if (randomValue < randomness && restaurantContext.GetAvailableChair() != null)
            {
                SpawnCustomer();
            }
        }
    }
    private void OnDisable() {
        restaurantContext.onRestaurantClosed.RemoveListener(OnRestaurantClosed);
    }

    private void OnRestaurantClosed()
    {
        // Stop spawning customers when the restaurant is closed
        enabled = false;
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