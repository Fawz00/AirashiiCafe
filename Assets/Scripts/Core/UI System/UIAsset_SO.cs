#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "UI/Typed Entry")]
public class UIAsset_SO : ScriptableObject
{
    public string uiIdentifier;
    public VisualTreeAsset uiAsset;

#if UNITY_EDITOR
    public MonoScript handlerScript;
#endif

    [HideInInspector]
    public string handlerClassName;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (handlerScript != null)
        {
            System.Type type = handlerScript.GetClass();
            if (type != null && typeof(UIScript).IsAssignableFrom(type))
            {
                handlerClassName = type.FullName;
            }
        }
#endif
    }

    #region Public Methods

    public void Show()
    {
        if (uiAsset != null)
        {
            var uiDocument = UIManager.Instance.uiDocument;
            if (uiDocument != null)
            {
                uiDocument.visualTreeAsset = uiAsset;
            }
        }
    }

    #endregion Public Methods
}
