using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data/Player Data")]
public class PlayerData_SO : ScriptableObject
{
    public string currentScene;
    public int coins = 0;

    public PlayerData_SO()
    {
       currentScene = Common.startupScene;
    }
}
