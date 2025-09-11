using UnityEngine;

public class Character : MonoBehaviour
{
    public Character_SO character;

    public GameObject spawnedCharacter { get; private set; } = null;

    void Awake()
    {
        if (character == null)
        {
            Debug.LogError("Character: No character data assigned!", this);
            return;
        }
        SpawnCharacter();
    }

    public void SpawnCharacter()
    {
        if (character == null) return;

        if (spawnedCharacter != null)
        {
            Destroy(spawnedCharacter);
        }

        if (character.characterPrefab != null)
        {
            spawnedCharacter = Instantiate(character.characterPrefab, transform);
            spawnedCharacter.name = character.name;
        }
        else
        {
            Debug.LogError("Character: Character prefab is null!", this);
        }
    }
    public void SetCharacter(Character_SO newCharacter)
    {
        character = newCharacter;
        SpawnCharacter();
    }
    public void SetCharacter(string newCharacter)
    {
        Character_SO newSO = Common.GetScriptableObjectFromResources<Character_SO>($"Characters/{newCharacter}");
        if (newSO != null)
        {
            SetCharacter(newSO);
        }
        else
        {
            Debug.LogError($"Character: Character SO with id '{newCharacter}' not found!", this);
        }
    }
    public string GetCharacterName()
    {
        return character != null ? character.name : "Unknown";
    }
    public Animator getAnimator()
    {
        if (spawnedCharacter == null && character != null)
        {
            SpawnCharacter();
        }
        return spawnedCharacter != null ? spawnedCharacter.GetComponent<Animator>() : null;
    }
}
