using UnityEngine;
using UnityEngine.UIElements;

public class UI_PauseMenu : UIScript
{
    private Button resumeButton;

    protected override void Awake()
    {
        base.Awake();
        resumeButton = uiDocument.rootVisualElement.Q<Button>("resumeButton");
    }

    void OnEnable()
    {
        resumeButton.clicked += OnResumeButtonClicked;
    }
    void OnDisable()
    {
        resumeButton.clicked -= OnResumeButtonClicked;
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("Resume button clicked!");
        UIManager.Instance.BackUI();
    }
}
