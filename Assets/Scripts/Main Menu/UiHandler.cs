using UnityEngine;
using UnityEngine.UIElements;

public class UiHandler : MonoBehaviour
{
    public UIDocument uiDocument => GetComponent<UIDocument>();
    private Button playButton;
    private Button quitButton;

    void Awake()
    {
        playButton = uiDocument.rootVisualElement.Q<Button>("playButton");
        quitButton = uiDocument.rootVisualElement.Q<Button>("quitButton");
    }

    void OnEnable()
    {
        playButton.clicked += OnPlayButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }
    void OnDisable()
    {
        playButton.clicked -= OnPlayButtonClicked;
        quitButton.clicked -= OnQuitButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(Common.startupScene);
    }
    private void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked!");
        Application.Quit();
    }
}
