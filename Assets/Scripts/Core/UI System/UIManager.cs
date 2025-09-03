using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIManager : Singleton<UIManager>
{
    public UIDocument uiDocument => GetComponent<UIDocument>();

    public UIAsset_SO popupUi;
    public UIAsset_SO pauseUi;
    public UIAsset_SO HUDUi;

    private Stack<UIStack> uiStack = new Stack<UIStack>();
    private System.Type lastHandlerType;
    private PlayerControllerBase playerControllerBase;
    private bool canBack = true;

    public override void Awake()
    {
        base.Awake();

        if(pauseUi == null) pauseUi = GetUiFromResource("pause_menu");
        if(popupUi == null) popupUi = GetUiFromResource("popup_dialog");
        if(HUDUi == null) HUDUi = GetUiFromResource("hud_ui");
    }
    private void Start()
    {
        playerControllerBase = GameManager.Instance.playerControllerBase;
        if (playerControllerBase == null)
        {
            Debug.LogError("PlayerControllerBase is not set. Please assign it in the inspector or through code.");
            return;
        }

        AddUI(HUDUi, false, true, true); // Add HUD UI to the stack
    }

    public void Update()
    {
        if (playerControllerBase.InputActions.UI.Back.triggered)
        {
            if (canBack)
            {
                BackUI(true);
            }
        }
    }

    public UIStack CurrentUI
    {
        get
        {
            if (uiStack.Count > 0)
            {
                return uiStack.Peek();
            }
            return null;
        }
    }
    public UIAsset_SO GetUiFromResource(string ui_id)
    {
        UIAsset_SO data = Resources.Load<UIAsset_SO>($"UI/{ui_id}");
        if (data == null)
        {
            Debug.LogError($"UIAsset with ID '{ui_id}' not found in Resources/UI.");
            return null;
        }
        return data;
    }
    public void ShowDialog(string message, string title = "Information", string buttonText = "OK")
    {
        if (popupUi == null)
        {
            Debug.LogError("Dialog UI is not set. Please assign it in the inspector or through code.");
            return;
        }

        AddUI(popupUi);

        var rootElement = uiDocument.rootVisualElement;
        rootElement.Q<Label>("title_text").text = title;
        rootElement.Q<Label>("message_text").text = message;
        rootElement.Q<Button>("ok_button").text = buttonText;
    }
    public void AddUI(UIAsset_SO ui, bool shouldPause = true, bool allowBack = true, bool allowPlayerControl = false)
    {
        if (ui != null)
        {
            uiStack.Push(new UIStack { uiAsset = ui, canBack = allowBack, shouldPause = shouldPause, allowPlayerControl = allowPlayerControl });
            RenderCurrentUI();
        }
        else
        {
            Debug.LogError("UI is null");
        }
    }
    public void BackUI(bool shouldAskQuitIfStackEmpty = false, int backStep = 1)
    {
        if (uiStack.Count > 1)
        {
            int steps = Mathf.Clamp(backStep, 1, uiStack.Count - 1);
            for (int i = 0; i < steps; i++)
            {
                // Remove the component script if it exists
                if (uiStack.Peek().uiAsset.handlerClassName != null && uiStack.Peek().uiAsset.handlerClassName != "")
                {
                    var handlerType = System.Type.GetType(uiStack.Peek().uiAsset.handlerClassName);
                    if (handlerType != null)
                    {
                        var component = gameObject.GetComponent(handlerType);
                        if (component != null)
                        {
                            // cast to UIScript to call OnClose if needed
                            if (component is UIScript uiScript)
                            {
                                uiScript.OnClose();
                            }
                            Destroy(component);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Handler class '{uiStack.Peek().uiAsset.handlerClassName}' not found.");
                    }
                }
                uiStack.Pop();
                if (uiStack.Count <= 1) break;
            }
            RenderCurrentUI();
        }
        else
        {
            if (shouldAskQuitIfStackEmpty)
            {
                // Pause the app
                AddUI(pauseUi);
            }
            else
            {
                // Reset to HUD UI
                uiStack.Clear();
                AddUI(HUDUi, false, true, true);
            }
        }
    }

    public void ClearUI(UIAsset_SO uIAsset = null)
    {
        uiStack.Clear();
        if (uIAsset != null)
        {
            // If a specific UIAsset is provided, add it back to the stack
            AddUI(uIAsset, false, true, true);
        }
        else
        {
            // Otherwise, reset to HUD UI
            AddUI(HUDUi, false, true, true);
        }
    }

    public T GetCurrentUIScript<T>() where T : Component
    {
        if (uiStack.Count > 0)
        {
            UIStack current = uiStack.Peek();
            var handlerType = System.Type.GetType(current.uiAsset.handlerClassName);
            if (handlerType != null)
            {
                var component = gameObject.GetComponent(handlerType);
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
                else
                {
                    Debug.LogError($"Component is not of type {typeof(T).FullName}, expected {handlerType.FullName}.");
                }
            }
            else
            {
                Debug.LogError($"Handler class '{current.uiAsset.handlerClassName}' not found.");
            }
        }
        return null;
    }

    private void RenderCurrentUI()
    {
        UIStack current = uiStack.Peek();
        current.uiAsset.Show();
        canBack = current.canBack;

        if (current.uiAsset.handlerClassName != null && current.uiAsset.handlerClassName != "")
        {
            var handlerType = System.Type.GetType(current.uiAsset.handlerClassName);
            if (handlerType != null)
            {
                if( lastHandlerType != null && lastHandlerType != handlerType)
                {
                    // Remove the previous handler if it exists
                    var previousHandler = gameObject.GetComponent(lastHandlerType);
                    if (previousHandler != null)
                    {
                        Destroy(previousHandler);
                    }
                }
                gameObject.AddComponent(handlerType);
                lastHandlerType = handlerType;
            }
            else
            {
                Debug.LogError($"Handler class '{current.uiAsset.handlerClassName}' not found.");
            }
        }

        if (current.shouldPause)
        {
            playerControllerBase.PauseGame(true);
        }
        else
        {
            playerControllerBase.PauseGame(false);
        }

        if (!current.allowPlayerControl)
        {
            playerControllerBase.SetPlayerControlType(Common.PlayerControlType.UIOnly);
        }
        else
        {
            playerControllerBase.SetPlayerControlType(Common.PlayerControlType.Dynamic);
        }
    }
}
