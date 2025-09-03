using System;
using System.Collections;
using System.Collections.Generic;
using Ink;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class InkDialogueBase : MonoBehaviour
{
    public UnityEvent<Story> OnCreateStory;
    [SerializeField] public UnityEvent<string, string[]> OnInkEvent;
    [SerializeField] public UnityEvent OnFinished;
    [SerializeField] public UnityEvent OnStoryStarted;

    [SerializeField] public TextAsset inkJSONAsset;
    [SerializeField] public InkDialogueImageLibrary imageLibrary;

    [SerializeField] private bool playOnStart;
    public Story story;
    public string speakerName;

    private VisualElement dialogueUIElement;
    private ListView dialogueOptionsListView;
    private List<Choice> optionsList = new List<Choice>();
    private Coroutine typingCoroutine;
    private bool autoPlay = false;
    private bool allowContinue = false;

    void Awake()
    {

    }

    void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(WaitAndStartDialogue());

        }
    }

    private IEnumerator WaitAndStartDialogue()
    {
        yield return null; // Wait for one frame, or adjust as needed
        StartDialogue();
    }
    public void StartDialogue()
    {
        setup();
        if (inkJSONAsset == null)
        {
            Debug.LogError("Ink JSON Asset is not assigned.");
            return;
        }

        story = new Story(inkJSONAsset.text);
        OnCreateStory?.Invoke(story);
        OnStoryStarted?.Invoke();
        story.onError += handleError;
        RefreshView();
    }

    private void setup()
    {
        UIManager.Instance.AddUI(UIManager.Instance.GetUiFromResource("dialogue"), false, false);
        dialogueUIElement = UIManager.Instance.uiDocument.rootVisualElement;

        dialogueOptionsListView = dialogueUIElement.Q<ListView>("dialogue-list");
        dialogueOptionsListView.itemsSource = optionsList;
        dialogueOptionsListView.bindItem = (element, i) =>
        {
            Debug.Log($"Binding choice {i}: {optionsList[i].text};");
            var button = element.Q<Button>("button");
            button.text = optionsList[i].text.Trim();

            int capturedIndex = i;
            if (button.userData is EventCallback<ClickEvent> oldCallback)
                button.UnregisterCallback(oldCallback);

            EventCallback<ClickEvent> newCallback = evt => OnClickChoiceButton(optionsList[capturedIndex]);

            // Daftarin handler baru
            button.RegisterCallback<ClickEvent>(newCallback);
            button.userData = newCallback;
        };

        // dialogueUI.SetActive(false);
        Button button = dialogueUIElement.Q<Button>("continue-button");

        EventCallback<ClickEvent> newCallback = evt => OnClickContinueButton();
        button.RegisterCallback(newCallback);
        button.userData = newCallback;
    }
    private void handleError(string message, ErrorType t)
    {
        UIManager.Instance.AddUI(UIManager.Instance.GetUiFromResource("dialogue"), false, false);

        dialogueUIElement.Q<Label>("text-dialogue").text = $"ERROR: {message}\nTYPE: {t.ToString()}";
        dialogueUIElement.Q<Label>("text-name").text = "$SYSTEM$";

        StartCoroutine(WaitForDuration(3.5f, () =>
            {
                ResetUI();
                UIManager.Instance.BackUI();
                OnFinished?.Invoke();
            }
        ));
    }

    public void setVariable(string variableName, string value)
    {
        story.variablesState[variableName] = value;
    }
    public void setVariable(string variableName, int value)
    {
        story.variablesState[variableName] = value;
    }
    public void setVariable(string variableName, float value)
    {
        story.variablesState[variableName] = value;
    }
    public void setVariable(string variableName, bool value)
    {
        story.variablesState[variableName] = value;
    }
    public string getVariable(string variableName)
    {
        return story.variablesState[variableName].ToString();
    }
    private void RefreshView()
    {
        // Reset UI
        ResetUI();
        if (story.canContinue)
        {
            string text = story.Continue();
            text = text.Trim();

            // Check if the text is empty or contains only whitespace
            if (string.IsNullOrWhiteSpace(text))
            {
                RefreshView();
                return;
            }

                string speaker = null;

            // Check if there are any tags in the story
            if (story.currentTags.Count > 0)
            {
                foreach (string tag in story.currentTags)
                {
                    switch (tag)
                    {
                        // Example: "# event:myEvent=param1,param2"
                        case string t when t.StartsWith("event:"):
                            {
                                string[] eventData = tag.Substring(6).Trim().Split("=");
                                string[] param = { };
                                if (eventData.Length > 1)
                                {
                                    param = eventData[1].Trim().Split(",");
                                }

                                OnInkEvent?.Invoke(eventData[0], param);
                                break;
                            }
                        // Example: "# speaker:John"
                        case string t when t.StartsWith("speaker:"):
                            {
                                speaker = tag.Substring(8).Trim();
                                break;
                            }
                        // Example: "# background:myBackground"
                        case string t when t.StartsWith("background:"):
                            {
                                string backgroundName = tag.Substring(11).Trim();
                                if (backgroundName == "none")
                                {
                                    dialogueUIElement.Q<VisualElement>("background").style.backgroundImage = new StyleBackground();
                                    break;
                                }

                                if (imageLibrary.ImageDictionary.TryGetValue(backgroundName, out Texture2D backgroundImage))
                                {
                                    dialogueUIElement.Q<VisualElement>("background").style.backgroundImage = new StyleBackground(backgroundImage);
                                }
                                else
                                {
                                    Debug.LogWarning($"Background image '{backgroundName}' not found in the dictionary.");
                                }
                                break;
                            }
                        // Example: "# face-left:myFaceLeft"
                        case string t when t.StartsWith("face-left:"):
                            {
                                string faceName = tag.Substring(10).Trim();
                                if (imageLibrary.ImageDictionary.TryGetValue(faceName, out Texture2D faceImage))
                                {
                                    dialogueUIElement.Q<VisualElement>("face-left").style.backgroundImage = new StyleBackground(faceImage);
                                }
                                else
                                {
                                    Debug.LogWarning($"Face image '{faceName}' not found in the dictionary.");
                                }
                                break;
                            }
                        // Example: "# face-right:myFaceRight"
                        case string t when t.StartsWith("face-right:"):
                            {
                                string faceName = tag.Substring(11).Trim();
                                if (imageLibrary.ImageDictionary.TryGetValue(faceName, out Texture2D faceImage))
                                {
                                    dialogueUIElement.Q<VisualElement>("face-right").style.backgroundImage = new StyleBackground(faceImage);
                                }
                                else
                                {
                                    Debug.LogWarning($"Face image '{faceName}' not found in the dictionary.");
                                }
                                break;
                            }
                    }
                }
            }
            if (string.IsNullOrEmpty(speaker))
            {
                speaker = speakerName;
            }

            // Typing effect
            var textComponent = dialogueUIElement.Q<Label>("text-dialogue");
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(textComponent, text, 0.05f));

            dialogueUIElement.Q<Label>("text-name").text = speaker;

            // Delay before closing the dialogue
            StartCoroutine(WaitForDuration(1.5f, () =>
            {
                // Display all the choices, if there are any!
                if (story.currentChoices.Count > 0)
                {
                    var choiceList = story.currentChoices;
                    // Display log
                    Debug.Log($"Choices available: {choiceList.Count}");
                    foreach (var choice in choiceList)
                    {
                        Debug.Log($"Choice: {choice.index} - {choice.text}");
                    }

                    CreateChoiceView(choiceList);
                    return;
                }
                else
                {
                    dialogueOptionsListView.itemsSource = null;
                    if (autoPlay)
                    {
                        RefreshView();
                        return;
                    }
                    else
                    {
                        allowContinue = true;
                    }
                }
            }));
        }
        else
        {
            ResetUI();

            // No more choices, so we can close the dialogue
            Button button = dialogueUIElement.Q<Button>("continue-button");
            if (button.userData is EventCallback<ClickEvent> oldCallback)
                button.UnregisterCallback(oldCallback);
            
            UIManager.Instance.BackUI();
            OnFinished?.Invoke();
        }
        
    }
    private void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }
    private void CreateChoiceView(List<Choice> choices)
    {
        if (dialogueOptionsListView == null)
        {
            Debug.LogError("Dialogue Options ListView is not assigned.");
            return;
        }

        optionsList = choices;
        dialogueOptionsListView.itemsSource = optionsList;
    }
    private void ResetUI()
    {
        if(dialogueUIElement == null)
        {
            Debug.LogError("Dialogue UI Element is not assigned.");
            return;
        }
        dialogueOptionsListView.itemsSource = null;
    }
    private void OnClickContinueButton()
    {
        if (allowContinue)
        {
            allowContinue = false;
            RefreshView();
        }
    }

    private IEnumerator TypeText(Label textComponent, string fullText, float typeSpeed = 0.03f)
    {
        textComponent.text = "";
        foreach (char c in fullText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    private IEnumerator WaitForDuration(float duration, Action onComplete = null)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
}
