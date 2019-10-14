using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class DialogueManager
{
    public string name;
    public NpcDialogue nPCdialogue;
    public List<PlayerAnswers> answers;
}
