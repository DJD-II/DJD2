using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Answer", menuName = "Answer")]
public class PlayerAnswers : ScriptableObject
{
    public int toID;
    public bool exit;
    public int cost;
    public int Intelligence;
    public string text = "APPLY_TEXT_FIELD";
    public List<Item> itemsToGive = new List<Item>();
    public List<Quest> questsToGive = new List<Quest>();

}

