using UnityEngine;

public enum SwitchType : byte
{
    Answers,
    Dialogue,
}

[System.Serializable]
public class NpcDialogue
{
    

    [SerializeField]
    private int toID = 0;
    [SerializeField]
    private SwitchType switchTo;
    [SerializeField]
    private string text = "APPLY_TEXT_FIELD";
   
    public SwitchType SwitchTo { get => switchTo; }
    public int ToID { get => toID; }
    public string Text { get => text; } 
}
