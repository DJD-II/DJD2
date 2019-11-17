using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
[CreateAssetMenu(fileName = "New Hack Menu", menuName = "Hack Menu")]
sealed public class Hack : ScriptableObject
{
    [System.Serializable]
    sealed public class Point
    {
        [System.Serializable]
        sealed public class Command
        {
            [Header("Base")]
            [SerializeField] private string text = "";
            [SerializeField] private int id = 0;
            [SerializeField] private int toId = 0;
            [Header("To Earn")]
            [SerializeField] private List<Quest> questsToEarn = null;
            [SerializeField] private List<Event> eventsToGive = null;
            [SerializeField] private List<Item> itemsToEarn = null;
            [Header("To Give")]
            [SerializeField] private List<Item> itemsToGive = null;
            [Header("To Complete")]
            [SerializeField] private List<Quest> questsToComplete = null;
            [Header("Requesites")]
            [SerializeField] private Requesite requisites = null;

            public string Text { get => text; }
            public int ID { get => id; }
            public int ToID { get => toId; }

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

        [SerializeField] private string name;
        [SerializeField] private string text = null;
        [SerializeField] private List<Command> commands = null;

        public string Text { get => text; }
        public List<Command> Commands { get => commands; }
    }

    [SerializeField] private List<Point> hackPoints = null;

    public List<Point> HackPoints { get => hackPoints; }
}
