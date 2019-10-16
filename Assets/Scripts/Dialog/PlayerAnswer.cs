using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Answer", menuName = "Dialogue/Answer")]
public class PlayerAnswer : ScriptableObject
{
    public int toID;
    public Command[] command = new Command[0];

    public int cost;
    public int Intelligence;
    public List<Item> itemsToGive = new List<Item>();
    public List<Quest> questsToGive = new List<Quest>();

    public string text = "APPLY_TEXT_FIELD";

    public enum Command
    {
        Quit,
        GiveItem,
        GiveQuest,
        SubtractMoney,
        AddMoney,

    }
}

