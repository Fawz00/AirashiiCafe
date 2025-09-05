using UnityEngine;

public class Common
{
    public static string startupScene = "BOOTSTRAP";
    public static string gameScene = "Gameplay";

    public enum PlayerControlType
    {
        GameOnly,
        UIOnly,
        Dynamic, // Automatically switches between Ground and Turn Based controls
        All,
        None, // No player control
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
    }

    public enum ItemType
    {
        Generic,
        Meal,
        Beverage,
        OtherConsumable,
    }

    public static T GetScriptableObjectFromResource<T>(string path_to_id) where T : ScriptableObject
    {
        T data = Resources.Load<T>(path_to_id);
        if (data == null)
        {
            Debug.LogError($"Scriptable Object of type '{typeof(T)}' with path '{path_to_id}' not found in Resources.");
            return null;
        }
        return data;
    }

    public static PlayerData_SO GetSavedData()
    {
        PlayerData_SO data = GetScriptableObjectFromResource<PlayerData_SO>("player_data");
        return data;
    }
}

