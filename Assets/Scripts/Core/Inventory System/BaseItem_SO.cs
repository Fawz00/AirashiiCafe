using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory System/Base Item")]
public abstract class BaseItem_SO : ScriptableObject
{
    [SerializeField] public new string name;
    [SerializeField] public string typeId;
    [SerializeField] [TextArea] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public abstract Common.ItemType itemType { get; }
    [SerializeField] public Common.Rarity rarity;
    [SerializeField] public bool isStackable = true;

    [NonSerialized] public string id;

    void Awake()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
