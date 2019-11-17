using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Conversation", menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public List<DialogueManager> dialogues = new List<DialogueManager>();
    [SerializeField]
    private List<AnswerHolder> answers = null;
    [SerializeField]
    private Requesite requisites = null;

    public List<AnswerHolder> Answers { get => answers; }

    public bool Fullfills(PlayerController controller)
    {
        return requisites.Fullfills(controller);
    }
}

[System.Serializable]
public class AnswerHolder
{
    [SerializeField]
    private string name;
    [SerializeField]
    private PlayerAnswer[] answers = null;

    public PlayerAnswer[] Answers { get => answers; }
}
