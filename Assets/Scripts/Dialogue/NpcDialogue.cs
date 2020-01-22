using UnityEngine;

[System.Serializable]
/// <summary>
/// Responsible for creating the NPC answers
/// </summary>
public class NpcDialogue
{
    // The id of the associated answer
    [SerializeField] private int toID = 0;
    // Creates a SwitchType and sets it to Answers
    [SerializeField] private SwitchType switchTo = SwitchType.Answers;
    // A string with the dialogue itself
    [SerializeField] private string text = "APPLY_TEXT_FIELD";
    // Associated audio
    [SerializeField] private AudioClip audio = null;

    /// <summary>
    /// Property to get the audio
    /// </summary>
    public AudioClip Audio { get => audio; }

    /// <summary>
    /// Property to get the SwitchType
    /// </summary>
    public SwitchType SwitchTo { get => switchTo; }

    /// <summary>
    /// Property to get the id
    /// </summary>
    public int ToID { get => toID; }

    /// <summary>
    /// Property to get the dialogue text
    /// </summary>
    public string Text { get => text; }
}
