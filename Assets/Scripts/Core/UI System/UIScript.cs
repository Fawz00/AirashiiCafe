using UnityEngine;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    protected UIDocument uiDocument;

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    public virtual void OnClose()
    {
        
    }
}
