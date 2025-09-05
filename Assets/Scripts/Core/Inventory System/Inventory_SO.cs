using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory")]
public class Inventory_SO : ScriptableObject
{
    private struct InventoryItem
    {
        public BaseItem_SO item;
        public int quantity;
        public InventoryItem(BaseItem_SO item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

    private List<InventoryItem> items = new();

    public void AddItem(BaseItem_SO item, int quantity = 1)
    {
        if (item.isStackable)
        {
            var existing = items.Find(i => i.item == item);
            if (!EqualityComparer<InventoryItem>.Default.Equals(existing, default))
            {
                existing.quantity += quantity;
                return;
            }
        }

        items.Add(new InventoryItem(item, quantity));
    }

    public void RemoveItem(BaseItem_SO item, int quantity = 1)
    {
        var existing = items.Find(i => i.item == item);
        if (!EqualityComparer<InventoryItem>.Default.Equals(existing, default))
        {
            existing.quantity -= quantity;
            if (existing.quantity <= 0)
            {
                if (existing.quantity < 0)
                    Debug.LogWarning($"Removed more items than available.");

                items.Remove(existing);
            }
        }
    }

    public int GetItemQuantity(BaseItem_SO item)
    {
        var existing = items.Find(i => i.item == item);
        return !EqualityComparer<InventoryItem>.Default.Equals(existing, default) ? existing.quantity : 0;
    }

    public List<(BaseItem_SO item, int quantity)> GetAllItems()
    {
        var itemList = new List<(BaseItem_SO item, int quantity)>();
        foreach (var invItem in items)
        {
            itemList.Add((invItem.item, invItem.quantity));
        }
        return itemList;
    }
}
