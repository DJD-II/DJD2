using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueManager
{
    public string name;
    public NPCdialogue nPCdialogue;
    public List<PLAYERanswer> answers;
}
