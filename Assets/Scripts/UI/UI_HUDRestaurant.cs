using System;
using Core.Events;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_HUDRestaurant : UIScript
{
    private Button pauseButton;
    private ProgressBar timerBar;
    private Button cookButton1;
    private Button cookButton2;
    private Button cookButton3;
    private VisualElement order1;
    private VisualElement order2;
    private VisualElement order3;
    private Label coins;

    protected override void Awake()
    {
        base.Awake();

        pauseButton = uiDocument.rootVisualElement.Q<Button>("pauseButton");
        timerBar = uiDocument.rootVisualElement.Q<ProgressBar>("timerBar");
        cookButton1 = uiDocument.rootVisualElement.Q<Button>("cookButton1");
        cookButton2 = uiDocument.rootVisualElement.Q<Button>("cookButton2");
        cookButton3 = uiDocument.rootVisualElement.Q<Button>("cookButton3");
        order1 = uiDocument.rootVisualElement.Q<VisualElement>("order1");
        order2 = uiDocument.rootVisualElement.Q<VisualElement>("order2");
        order3 = uiDocument.rootVisualElement.Q<VisualElement>("order3");
        coins = uiDocument.rootVisualElement.Q<Label>("coins");
    }
    void OnEnable()
    {
        pauseButton.clicked += OnPauseButtonClicked;
        EventBus.Subscribe<Event_OnInventoryUpdated>(OnInventoryUpdated);
    }

    void OnDisable()
    {
        pauseButton.clicked -= OnPauseButtonClicked;
        EventBus.Unsubscribe<Event_OnInventoryUpdated>(OnInventoryUpdated);
    }

    private void OnInventoryUpdated(Event_OnInventoryUpdated updated)
    {
        coins.text = GameManager.Instance.playerData.inventory.GetItemQuantity("coin").ToString();
    }

    void LateUpdate()
    {
        int coinData = GameManager.Instance.playerData.inventory.GetItemQuantity("coin");
        coins.text = coinData.ToString();
    }

    private void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked!");
        UIManager.Instance.BackUI(true);
    }
}
