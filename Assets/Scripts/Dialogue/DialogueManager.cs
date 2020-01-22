using UnityEngine;

[System.Serializable]
/// <summary>
/// Responsible for holding the NPC answers
/// </summary>
public class DialogueManager
{
    // Creates a string with the name 
    [SerializeField] private string name;
    // Creates an array of NpcDialogue
    [SerializeField] private NpcDialogue dialogue = null;

    /// <summary>
    /// Creates a property to get the dialogue
    /// </summary>
    public NpcDialogue Dialogue { get => dialogue; }
}
