using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NPCController))]
[RequireComponent(typeof(FollowPath))]
public class Customer : MonoBehaviour
{
    [Header("Restaurant Settings")]
    public RestaurantContext restaurantContext;

    [Header("Customer Settings")]
    public string customerName = "Customer";
    [Range(5, 20)] public float eatTime = 5f; // Time taken to eat
    public float patienceWaiting = 5f; // Current waiting time

    [Header("Customer Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private FollowPath followPath;
    [SerializeField] private NPCController npcController;

    [Header("Bubble Button Settings")]
    public UI_CustomerBubble customerButtonBubble;

    public bool isAlreadyEntered { get; private set; } = false;

    private Chair currentChair;
    private MenuItem_SO orderedItem;

    private int satisfaction = 3; // Customer satisfaction level (0-3)

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (npcController == null) npcController = GetComponent<NPCController>();
        if (followPath == null) followPath = GetComponent<FollowPath>();
        if (restaurantContext == null) restaurantContext = FindFirstObjectByType<RestaurantContext>();
        if (customerButtonBubble == null) customerButtonBubble = GetComponent<UI_CustomerBubble>();
    }
    void Start()
    {
        if (restaurantContext == null) restaurantContext = FindFirstObjectByType<RestaurantContext>();

        Chair chair = restaurantContext.GetAvailableChair();
        chair?.Reserve(this);
        if (chair != null && !chair.isOccupied && chair.reservedFor == this)
        {
            currentChair = chair;
            SearchForChair();
        }
        else
        {
            Debug.LogWarning("Customer: No available chairs found!");
        }
    }
    private void OnEnable() {
        customerButtonBubble.onFoodOrdered.AddListener(OnFoodOrdered);
        customerButtonBubble.onFoodNotOrdered.AddListener(OnFoodNotOrdered);
    }
    private void OnDisable() {
        customerButtonBubble.onFoodOrdered.RemoveListener(OnFoodOrdered);
        customerButtonBubble.onFoodNotOrdered.RemoveListener(OnFoodNotOrdered);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Chair>(out Chair chair);
        if (chair != null)
        {
            if (chair == currentChair) SitOnChair();
        }
    }

    private void SearchForChair()
    {
        followPath.endNode = currentChair.gameObject;
        followPath.FindNewPath();
    }
    private void SitOnChair()
    {
        if (animator != null)
        {
            animator.SetBool("isSitting", true);
        }

        currentChair.Sit(this);

        npcController.movement = Vector2.zero;
        transform.position = currentChair.transform.position;
        npcController.direction = currentChair.transform.right; // Face the same direction as the chair

        if (!isAlreadyEntered)
        {
            isAlreadyEntered = true;
            OrderFood();
        }
    }
    private void OrderFood()
    {
        // Logic to order food from the menu
        if (restaurantContext.menuItems.Count > 0)
        {
            int randomIndex = Random.Range(0, restaurantContext.menuItems.Count);
            MenuItem_SO orderedItem = restaurantContext.menuItems[randomIndex];
            this.orderedItem = orderedItem;

            // Show the bubble button UI
            customerButtonBubble.ShowOrderFoodBubble(orderedItem.item, patienceWaiting);
            Debug.Log($"{customerName} ordered: {orderedItem.name}");
        }
        else
        {
            Debug.LogWarning("Customer: No menu items available to order.");
            LeaveRestaurant();
        }
    }
    private void OnFoodOrdered()
    {
        Debug.Log($"{customerName} has ordered food.");
        satisfaction = Random.Range(1, 3); // Random satisfaction level for demo purposes
        // Logic to handle food ordering
        Invoke(nameof(LeaveRestaurant), eatTime); // Simulate eating time before leaving
    }
    private void OnFoodNotOrdered()
    {
        Debug.Log($"{customerName} did not order food and is leaving.");
        satisfaction = 0; // Customer is unhappy
        LeaveRestaurant();
    }
    private void LeaveRestaurant()
    {
        if (animator != null)
        {
            animator.SetBool("isSitting", false);
        }
        if (orderedItem != null) restaurantContext.AddIncome(orderedItem.price);
        currentChair.StandUp(this);

        orderedItem = null;
        currentChair = null;
        followPath.endNode = restaurantContext.entrance.gameObject;
        followPath.FindNewPath();
    }
}