using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public string label;
    public bool onlyInPlayMode;

    public ButtonAttribute(string label = "", bool onlyInPlayMode = false)
    {
        this.label = label;
        this.onlyInPlayMode = onlyInPlayMode;
    }
}
