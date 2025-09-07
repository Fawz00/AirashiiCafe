using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Customer))]
public class UI_CustomerBubble : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onFoodOrdered;
    public UnityEvent onFoodNotOrdered;

    [Header("References")]
    [SerializeField] private RestaurantContext restaurantContext;
    [SerializeField] private Customer customer;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private Slider slider;

    private float currentWaitTime = 10f;
    private float waitTime = 10f;

    private void Awake()
    {
        if (restaurantContext == null)
        {
            restaurantContext = FindFirstObjectByType<RestaurantContext>();
        }
        if (customer == null)
        {
            customer = GetComponent<Customer>();
        }
        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();
        }
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }
        if (image == null)
        {
            image = GetComponentInChildren<Image>();
        }
    }
    private void Start()
    {
        HideBubble();
    }
    private void Update()
    {
        // Don't update if the game is paused
        if (Time.timeScale == 0f)
            return;

        // Update the slider value based on remaining wait time
        if (canvas != null && canvas.enabled && slider != null)
        {
            currentWaitTime -= Time.unscaledDeltaTime;
            slider.value = Mathf.Clamp01(currentWaitTime / waitTime);

            // Update the slider color based on remaining time
            if (slider.value > 0.6f)
            {
                slider.fillRect.GetComponent<Image>().color = Color.green;
            }
            else if (slider.value > 0.35f)
            {
                slider.fillRect.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                slider.fillRect.GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void ShowOrderFoodBubble(BaseItem_SO item, float waitTime)
    {
        if (customer == null || canvas == null || button == null || image == null)
        {
            Debug.LogWarning("UI_CustomerBubble: Missing references.");
            return;
        }

        // Set the bubble image to the item's sprite
        if (item != null && item.icon != null)
        {
            image.sprite = item.icon;
        }
        else
        {
            Debug.LogWarning("UI_CustomerBubble: Item or item sprite is null.");
        }
        this.waitTime = waitTime;
        currentWaitTime = waitTime;
        slider.value = 1f; // Reset slider to full

        // Enable the canvas to show the bubble
        canvas.enabled = true;

        // Automatically hide the bubble after waitTime seconds
        Invoke(nameof(FoodNotOrdered), waitTime);

        // Assign the button click event
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Maid maid = restaurantContext.GetAvailableMaid();
            if (maid != null)
            {
                maid.AttendTarget(customer.gameObject, Common.ServiceType.TakeOrder);
                maid.onTaskFinished.AddListener(() =>
                {
                    onFoodOrdered?.Invoke();

                    maid.onTaskFinished.RemoveAllListeners();
                });

                // Cancel food not ordered invocation if the button is clicked
                CancelInvoke(nameof(FoodNotOrdered));
                HideBubble();
            }
        });
    }

    private void FoodNotOrdered()
    {
        onFoodNotOrdered.Invoke();
        HideBubble();
    }

    public void HideBubble()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}