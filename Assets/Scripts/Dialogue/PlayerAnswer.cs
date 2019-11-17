using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnswer
{
    [Header("Base")]
    [SerializeField] private string name;
    [Tooltip("The answer Text. This will be displayed in a button to choose from.")]
    [SerializeField] private string text = "APPLY_TEXT_FIELD";
    [Tooltip("For tracking purposes. When this answer is chosen this id will propegate" +
        " in a event.")]
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

    public int ID { get { return id; } }
    public SwitchType SwitchTo { get => switchTo; }
    public string Text { get => text; }
    public int ToID { get => toID; }

    public bool Fullfills(PlayerController controller)
    {
        return requisites.Fullfills(controller);
    }

    public void Process(PlayerController controller)
    {
        GameInstance.GameState.QuestController.CompleteQuests(questsToComplete);
        GameInstance.GameState.QuestController.Add(questsToEarn);
        controller.Inventory.Remove(itemsToGive);
        controller.Inventory.Add(itemsToEarn);
        GameInstance.GameState.EventController.Add(eventsToGive);
    }
}