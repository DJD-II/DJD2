using UnityEngine;

[System.Serializable]
/// <summary>
/// Responsible for holding the players answers
/// </summary>
public class AnswerHolder
{
    // Creates a string with the name 
    [SerializeField] private string name;
    // Creates an array of PlayerAnswer
    [SerializeField] private PlayerAnswer[] answers = null;

    /// <summary>
    /// Creates a property to get the answers
    /// </summary>
    public PlayerAnswer[] Answers { get => answers; }
}

