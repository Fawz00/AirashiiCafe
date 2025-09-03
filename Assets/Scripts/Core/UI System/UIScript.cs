using UnityEngine;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    protected UIDocument uiDocument;
    private bool closed = false;

    internal bool IsAlreadyClosed
    {
        get => closed;
        set => closed = value;
    }

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
    }

    public virtual void OnClose()
    {
        closed = true;
    }
}
