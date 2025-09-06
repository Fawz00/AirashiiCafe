using UnityEngine;

[RequireComponent(typeof(NPCController))]
[RequireComponent(typeof(FollowPath))]
public class Customer : MonoBehaviour
{
    [Header("Restaurant Settings")]
    public RestaurantContext restaurantContext;

    [Header("Customer Settings")]
    public string customerName = "Customer";
    public int patience = 100; // How long the customer is willing to wait
    [Range(5, 12)] public float eatTime = 5f; // Time taken to eat
    public float patienceWaiting = 5f; // Current waiting time


    [Header("Customer Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private FollowPath followPath;
    [SerializeField] private NPCController npcController;

    private bool isHungry = true;
    private int satisfaction = 3; // Customer satisfaction level (0-3)

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (npcController == null) npcController = GetComponent<NPCController>();
        if (followPath == null) followPath = GetComponent<FollowPath>();
    }
    void Start()
    {
        if (restaurantContext == null) restaurantContext = FindFirstObjectByType<RestaurantContext>();

        Chair chair = restaurantContext.GetAvailableChair();
        chair?.Reserve(this);
        if (chair != null && !chair.isOccupied && chair.reservedFor == this)
        {
            SearchForChair(chair);
        }
        else
        {
            Debug.LogWarning("Customer: No available chairs found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Chair>(out Chair chair);
        if (chair != null)
        {
            float duration = Random.Range(eatTime-4f, eatTime+4f);
            SitOnChair(chair, duration); // Sit for a random duration between 3 to 7 seconds
        }
    }

    public void LeaveRestaurant()
    {
        followPath.endNode = restaurantContext.entrance.gameObject;
        followPath.FindNewPath();
    }
    public void SearchForChair(Chair chair)
    {
        followPath.endNode = chair.gameObject;
        followPath.FindNewPath();
    }
    public void SitOnChair(Chair chair, float duration)
    {
        if (animator != null)
        {
            animator.SetBool("isSitting", true);
        }

        chair.Sit(this);

        npcController.movement = Vector2.zero;
        transform.position = chair.transform.position;
        npcController.direction = chair.transform.right; // Face the same direction as the chair

        // Start a coroutine to stand up after the specified duration
        StartCoroutine(StandUpAfterDelay(chair, duration));
    }
    private System.Collections.IEnumerator StandUpAfterDelay(Chair chair, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (animator != null)
        {
            animator.SetBool("isSitting", false);
        }
        chair.StandUp(this);
        LeaveRestaurant();
    }
}