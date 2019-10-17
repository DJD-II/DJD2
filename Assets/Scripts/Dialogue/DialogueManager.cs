using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueManager
{
    [SerializeField]
    private string name;
    [SerializeField]
    private NpcDialogue dialogue = null;

    public NpcDialogue Dialogue { get => dialogue; }
}
