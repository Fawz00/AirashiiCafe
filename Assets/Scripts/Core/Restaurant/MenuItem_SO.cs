using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Restaurant/Menu")]
public class MenuItem_SO : ScriptableObject
{
    public BaseItem_SO item;
    public int price;
    public List<BaseItem_SO> ingredients = new List<BaseItem_SO>();
    
    public MenuItem_SO()
    {

    }
}
