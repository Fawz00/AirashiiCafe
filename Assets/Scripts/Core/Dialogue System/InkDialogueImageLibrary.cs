using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/InkDialogueImageLibrary")]
public class InkDialogueImageLibrary : ScriptableObject
{
    [Header("Dialogue Images")]
    [SerializeField]
    private List<DialogueImageEntry> imageEntries = new List<DialogueImageEntry>();

    private Dictionary<string, Texture2D> imageDictionary;

    // Runtime-accessible dictionary (lazy-loaded)
    public Dictionary<string, Texture2D> ImageDictionary
    {
        get
        {
            if (imageDictionary == null)
            {
                imageDictionary = new Dictionary<string, Texture2D>();
                foreach (var entry in imageEntries)
                {
                    if (!imageDictionary.ContainsKey(entry.characterName) && entry.texture != null)
                    {
                        imageDictionary.Add(entry.characterName, entry.texture);
                    }
                }
            }
            return imageDictionary;
        }
    }

    [Serializable]
    public class DialogueImageEntry
    {
        public string characterName;
        public Texture2D texture;
    }
}
