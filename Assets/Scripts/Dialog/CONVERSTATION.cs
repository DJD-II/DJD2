using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Conversation", menuName = "Conversation")]
public class CONVERSATION : ScriptableObject
{
    public List<DialogueManager> managerContents = new List<DialogueManager>();


}
