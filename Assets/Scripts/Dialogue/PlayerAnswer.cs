using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Responsible for creating the player dialogue
/// </summary>
public class PlayerAnswer
{
    [Header("Base")]
    // For tracking porpouses withing unity engine
    [SerializeField] private string name = default;

    [Tooltip("The answer Text. This will be displayed in a button to choose from.")]
    [SerializeField] private string text = "APPLY_TEXT_FIELD";

    [Tooltip("For tracking purposes. When this answer is chosen this id will " +
        "propegate in a event.")]
    [SerializeField] private int id = 0;

    [Tooltip("Choose to what menu this answer will transition to.")]
    [SerializeField] SwitchType switchTo = SwitchType.Dialogue;

    [Tooltip("Based on 'Switch To' this will transition to the id of the menu. EXAMPLE : " +
        "SwitchTo : Dialogue; id = 0; -> this will switch to dialogues at slot 0\n"+
        "SwitchTo : Anwers; id = 2; -> This will switch to answers at slot 2.")]
    [SerializeField] private int toID = 0;

    [Header("To Earn")]
    [Tooltip("The items the player will earn if he has chosen this answer.")]
    [SerializeField] private List<Item> itemsToEarn = new List<Item>();

    [Tooltip("The quests the player will earn if he has chosen this answer.")]
    [SerializeField] private List<Quest> questsToEarn = new List<Quest>();

    [Tooltip("The items the player will earn if he has chosen this answer.")]
    [SerializeField] private List<Event> eventsToGive = new List<Event>();

    [Header("To Give")]
    [Tooltip("The items the player will lose if he has chosen this answer.")]
    [SerializeField] private List<Item> itemsToGive = new List<Item>();

    [Header("To Complete")]
    [Tooltip("The quests the player will complete if he has chosen this answer.")]
    [SerializeField] private List<Quest> questsToComplete = new List<Quest>();

    [Header("Requesites")]
    [Tooltip("The requesites needed for this answer to show up as a possible answer.")]
    [SerializeField] private Requesite requisites = null;

    /// <summary>
    /// Property to get the id
    /// </summary>
    public int ID { get => id;  }
    /// <summary>
    /// Property to get the SwitchType
    /// </summary>
    public SwitchType SwitchTo { get => switchTo; }
    /// <summary>
    /// Property to get the id it should jump to
    /// </summary>
    public int ToID { get => toID; }
    /// <summary>
    /// Property to get the dialogue text
    /// </summary>
    public string Text { get => text; }

    /// <summary>
    /// Checks if the player fills the requesits
    /// </summary>
    /// <param name="controller"> The player controller </param>
    /// <returns> A bool if it fills the requesits or not </returns>
    public bool Fullfills(PlayerController controller) =>
        requisites.Fullfills(controller);

    /// <summary>
    /// -------------------------------- < 
    /// </summary>
    /// <param name="controller"></param>
    public void Process(PlayerController controller)
    {
        // When the player Gives out an answer 
        GameInstance.GameState.QuestController.CompleteQuests(questsToComplete);
        // There are a few "events" to be given out or taken
        GameInstance.GameState.QuestController.Add(questsToEarn);
        // relative to the answers settings in the editor
        controller.Inventory.Remove(itemsToGive);
        // this method takes care of that.
        controller.Inventory.Add(itemsToEarn);
        // It adds quests, completes quests, adds items or events.
        GameInstance.GameState.EventController.Add(eventsToGive);
    }
}