using System.Collections.Generic;
using UnityEngine;

// Creates a sub-menu to create conversations
[CreateAssetMenu(fileName = "Create Conversation", menuName = "Dialogue/Conversation")]

/// <summary>
/// Responsible for holding NPC's and players dialogues
/// </summary>
public class Conversation : ScriptableObject
{
    /// <summary>
    /// Creates a list to hold NPC dialogues
    /// </summary>
    public List<DialogueManager> dialogues = new List<DialogueManager>();

    /// <summary>
    /// Creates a list to hold player dialogues
    /// </summary>
    public List<AnswerHolder> Answers { get => answers; }

    // Creates a AnswerHolder list
    [SerializeField] private List<AnswerHolder> answers = null;
    // Creates a Requesite variable
    [SerializeField] private Requesite requisites = null;

    /// <summary>
    /// Checks if the player fills the requesits
    /// </summary>
    /// <param name="controller"> The player controller </param>
    /// <returns> A bool if it fills the requesits or not </returns>
    public bool Fullfills(PlayerController controller) => 
        requisites.Fullfills(controller);
}
