using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character_SO")]
public class Character_SO : ScriptableObject
{
    [Header("Character Info")]
    public string id;
    public Sprite characterIcon;
    public GameObject characterPrefab;
    public string characterName = "Character";
    [TextArea] public string description = "Character Description";
    public int age = 18;

    [Tooltip("Height in cm and Weight in kg")]
    public float height = 170f; // in cm
    public float weight = 70f; // in kg

    public Common.Rarity rarity = Common.Rarity.Common;

    [Header("Character Stats")]
    [Range(0, 100)] public int sleepy = 50;
    [Range(0, 100)] public int energy = 50;
    [Tooltip("Care level from -100 (Careless) to 100 (OKAY)")]
    [Range(-100, 100)] public int care = 50;
}
