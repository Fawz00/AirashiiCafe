using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<Character_SO> availableCharacters = new List<Character_SO>();
    public Character_SO currentCharacter;

    public GameObject spawnedCharacter { get; private set; } = null;

    void Awake()
    {
        // If availableCharacters is not empty, select randomly from it
        if (availableCharacters != null && availableCharacters.Count > 0)
        {
            int randomIndex = Random.Range(0, availableCharacters.Count);
            currentCharacter = availableCharacters[randomIndex];
        }

        SpawnCharacter();
    }

    public void SpawnCharacter()
    {
        if (currentCharacter == null)
        {
            Debug.LogError("Character: No character data assigned!", this);
            return;
        }

        if (spawnedCharacter != null)
        {
            Destroy(spawnedCharacter);
        }

        if (currentCharacter.characterPrefab != null)
        {
            spawnedCharacter = Instantiate(currentCharacter.characterPrefab, transform);
            spawnedCharacter.name = currentCharacter.name;
        }
        else
        {
            Debug.LogError("Character: Character prefab is null!", this);
        }
    }
    public void SetCharacter(Character_SO newCharacter)
    {
        currentCharacter = newCharacter;
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
        return currentCharacter != null ? currentCharacter.name : "Unknown";
    }
    public Animator getAnimator()
    {
        if (spawnedCharacter == null && currentCharacter != null)
        {
            SpawnCharacter();
        }
        return spawnedCharacter != null ? spawnedCharacter.GetComponent<Animator>() : null;
    }
}
