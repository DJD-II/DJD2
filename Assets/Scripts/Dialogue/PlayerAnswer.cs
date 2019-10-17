using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnswer
{
    [SerializeField]
    private string name;
    [SerializeField]
    SwitchType switchTo = SwitchType.Dialogue;
    public int toID;
    
    public int cost;
    public List<Item> itemsToGive = new List<Item>();
    public List<Quest> questsToGive = new List<Quest>();

    public string text = "APPLY_TEXT_FIELD";

    [SerializeField]
    private ConversationRequesite requisites = null;
    public ConversationRequesite Requesites { get => requisites; }
    public SwitchType SwitchTo { get => switchTo; }
}

