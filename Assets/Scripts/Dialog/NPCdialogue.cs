using UnityEngine;

[System.Serializable]
public class NpcDialogue
{
    [SerializeField]
    private int toAnswerID = 0;
    [SerializeField]
    private string text = "APPLY_TEXT_FIELD";
   
    public int ToAnswerID { get => toAnswerID; }
    public string Text { get => text; } 
}
