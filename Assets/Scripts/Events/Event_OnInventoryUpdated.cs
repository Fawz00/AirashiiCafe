using UnityEngine;

public struct Event_OnInventoryUpdated
{
    public Common.UpdateType updateType;

    public Event_OnInventoryUpdated(Common.UpdateType updateType)
    {
        this.updateType = updateType;
    }
    
}
