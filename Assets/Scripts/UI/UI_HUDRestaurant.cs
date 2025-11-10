using System;
using Core.Events;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_HUDRestaurant : UIScript
{
    private Button pauseButton;
    private ProgressBar timerBar;
    private Label days;
    private Label coins;

    private RestaurantContext restaurantContext;

    protected override void Awake()
    {
        base.Awake();

        restaurantContext = FindFirstObjectByType<RestaurantContext>();

        pauseButton = uiDocument.rootVisualElement.Q<Button>("pauseButton");
        timerBar = uiDocument.rootVisualElement.Q<ProgressBar>("time_bar");
        days = uiDocument.rootVisualElement.Q<Label>("days");
        coins = uiDocument.rootVisualElement.Q<Label>("coins");
    }
    void Start()
    {
        if (restaurantContext == null)
        {
            restaurantContext = FindFirstObjectByType<RestaurantContext>();
            if (restaurantContext == null)
            {
                Debug.LogError("UI_HUDRestaurant: RestaurantContext not found in the scene.");
                return;
            }
        }

        coins.text = restaurantContext.incomeToday.ToString();
    }
    void Update()
    {
        if (restaurantContext == null)
        {
            Debug.LogError("UI_HUDRestaurant: RestaurantContext is not assigned.");
            return;
        }
        if (timerBar == null) Debug.LogError("UI_HUDRestaurant: TimerBar is not assigned.");
        timerBar.value = restaurantContext.remainingOpenTime / restaurantContext.openDuration;
    }
    void OnEnable()
    {
        pauseButton.clicked += OnPauseButtonClicked;
        EventBus.Subscribe<Event_OnInventoryUpdated>(OnInventoryUpdated);
    }

    void OnDisable()
    {
        pauseButton.clicked -= OnPauseButtonClicked;
        restaurantContext.onIncomeAdded.RemoveListener(OnIncomeAdded);
        restaurantContext.onRestaurantClosed.RemoveListener(OnRestaurantClosed);
        EventBus.Unsubscribe<Event_OnInventoryUpdated>(OnInventoryUpdated);
    }

    public void SetRestaurantContext(RestaurantContext context)
    {
        restaurantContext = context;
        restaurantContext.onIncomeAdded.AddListener(OnIncomeAdded);
        restaurantContext.onRestaurantClosed.AddListener(OnRestaurantClosed);
        days.text = $"Day {restaurantContext.currentDay}";
    }

    private void OnRestaurantClosed()
    {
        timerBar.title = "Closed";
    }

    private void OnInventoryUpdated(Event_OnInventoryUpdated updated)
    {
        // Update cook buttons based on inventory changes
    }

    void OnIncomeAdded(int amount)
    {
        int coinsToday = restaurantContext.incomeToday;
        coins.text = coinsToday.ToString();
    }

    private void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked!");
        UIManager.Instance.BackUI(true);
    }
}
