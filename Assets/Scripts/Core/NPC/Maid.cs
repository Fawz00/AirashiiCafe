using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(NPCController))]
[RequireComponent(typeof(FollowPath))]
public class Maid : MonoBehaviour
{
    [Header("Maid Events")]
    public UnityEvent onTaskFinished;

    [Header("Restaurant Settings")]
    public RestaurantContext restaurantContext;

    [Header("Maid Settings")]
    public string customerName = "Maid";
    [Range(0, 10)] public int sleepy = 10; // How sleepy the maid is (0-10)
    public PathFindNode homePosition;

    [Header("Maid Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private FollowPath followPath;
    [SerializeField] private NPCController npcController;

    public bool isBusy { get; private set; } = false;
    private GameObject currentAttention = null;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (npcController == null) npcController = GetComponent<NPCController>();
        if (followPath == null) followPath = GetComponent<FollowPath>();
        if (restaurantContext == null) restaurantContext = FindFirstObjectByType<RestaurantContext>();
    }
    void Start()
    {
        if (restaurantContext == null) restaurantContext = FindFirstObjectByType<RestaurantContext>();

        if (homePosition != null)
        {
            followPath.endNode = homePosition.gameObject;
            followPath.FindNewPath();
        }

        currentAttention = null;
    }

    private void TaskFinished()
    {
        onTaskFinished?.Invoke();

        if (homePosition != null)
        {
            followPath.endNode = homePosition.gameObject;
            followPath.FindNewPath();
        }
        currentAttention = null;
        isBusy = false;
    }

    public void AttendTarget(GameObject target, Common.ServiceType serviceType)
    {
        if (target == null)
        {
            Debug.LogWarning("Maid: No customer to attend.");
            return;
        }
        if (isBusy)
        {
            Debug.LogWarning("Maid: Currently busy.");
            return;
        }
        isBusy = true;
        currentAttention = target;

        switch (serviceType)
        {
            case Common.ServiceType.InPlace:
                {
                    followPath.endNode = target;
                    followPath.FindNewPath();

                    followPath.onPathCompleted.AddListener((start, end) =>
                    {
                        if (end == target)
                        {
                            Debug.Log("Maid: Attending to target in place.");
                            // Here you can add code to interact with the customer, e.g., show a dialog, take order, etc.
                            // After finishing the task, call TaskFinished

                            // Wait for 2 seconds to simulate interaction
                            Invoke(nameof(TaskFinished), 2f);
                            followPath.onPathCompleted.RemoveAllListeners();
                        }
                    });
                    return;
                }
            case Common.ServiceType.TakeOrder:
                {
                    followPath.endNode = restaurantContext.kitchen.gameObject;
                    followPath.FindNewPath();

                    followPath.onPathCompleted.AddListener((start, end) =>
                    {
                        if (end == restaurantContext.kitchen.gameObject)
                        {
                            Debug.Log("Maid: Taking order to the kitchen.");
                            // Here you can add code to interact with the kitchen, e.g., deliver order, etc.
                            // After finishing the task, call TaskFinished

                            // Wait for 2 seconds to simulate interaction
                            Invoke(nameof(ServeOrder), 0.5f);
                            followPath.onPathCompleted.RemoveAllListeners();
                        }
                    });
                    return;
                }
            default:
                Debug.LogWarning("Maid: Unknown service type.");
                return;
        }
    }
    private void ServeOrder()
    {
        if (currentAttention != null)
        {
            Debug.Log($"Maid: Serving order to {currentAttention}.");

            followPath.endNode = currentAttention.gameObject;
            followPath.FindNewPath();

            followPath.onPathCompleted.AddListener((start, end) =>
            {
                if (end == currentAttention.gameObject)
                {
                    Invoke(nameof(TaskFinished), 2f);
                    followPath.onPathCompleted.RemoveAllListeners();
                }
            });
        }
    }
}