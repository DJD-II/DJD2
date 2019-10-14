using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Conversation", menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public List<DialogueManager> managerContents = new List<DialogueManager>();
}
