using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/Player Data")]
public class PlayerData_SO : ScriptableObject
{
    [NonSerialized] public string currentScene;
    [SerializeField] public Inventory_SO inventory;
    [SerializeField] public int day = 0;
    
    public PlayerData_SO()
    {
        currentScene = Common.startupScene;
    }
}
