using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RestaurantExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Customer customer = collision.GetComponent<Customer>();
        if (customer != null)
        {
            if (customer.isAlreadyEntered)
            {
                Destroy(customer.gameObject);
                Debug.Log($"{customer.customerName} has exited the restaurant and is removed from the game.");
            }
        }
    }
}