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
        MealSet,
        OtherConsumable,
    }

    public enum UpdateType
    {
        Add,
        Remove,
        Set,
    }

    public enum ServiceType
    {
        InPlace,
        TakeOrder
    }

    public static T GetScriptableObjectFromResources<T>(string path_to_id) where T : ScriptableObject
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
        PlayerData_SO data = GetScriptableObjectFromResources<PlayerData_SO>("player_data");
        return data;
    }
}

