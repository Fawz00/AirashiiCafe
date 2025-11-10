using UnityEngine;
using UnityEngine.UIElements;

public class UI_DaySummary : UIScript
{
    private Button continueButton;

    protected override void Awake()
    {
        base.Awake();
        continueButton = uiDocument.rootVisualElement.Q<Button>("continueButton");
    }

    void OnEnable()
    {
        continueButton.clicked += OnContinueButtonClicked;
    }
    void OnDisable()
    {
        continueButton.clicked -= OnContinueButtonClicked;
    }

    private void OnContinueButtonClicked()
    {
        LevelManager.GlobalResetAndLoad(Common.mainMenuScene);
    }
}
