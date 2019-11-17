using UnityEngine;



[System.Serializable]
public class NpcDialogue
{
    [SerializeField]
    private int toID = 0;
    [SerializeField]
    private SwitchType switchTo = SwitchType.Answers;
    [SerializeField]
    private string text = "APPLY_TEXT_FIELD";
    [SerializeField]
    private AudioClip audio = null;

    public AudioClip Audio { get => audio; }
    public SwitchType SwitchTo { get => switchTo; }
    public int ToID { get => toID; }
    public string Text { get => text; }
}
