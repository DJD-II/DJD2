using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed public class QuestController : object, ISavable
{
    [System.Serializable]
    public struct QuestInfo
    {
        private string name;
        private bool completed;

        public string Name { get { return name; } }
        public bool Completed { get { return completed; } }

        public QuestInfo(QuestID id)
        {
            name = id.quest.name;
            completed = id.completed;
        }
    }

    sealed public class QuestID
    {
        public delegate void EventHandler(QuestID sender);

        public event EventHandler OnCompleted;

        public Quest quest;
        public bool completed;

        public QuestID (QuestInfo info)
        {
            quest = QuestUtility.Get(info.Name);
            completed = info.Completed;
        }

        public QuestID(Quest quest)
        {
            this.quest = quest;
        }

        public void Complete()
        {
            if (completed)
                return;

            completed = true;

            OnCompleted?.Invoke(this);

            foreach (Quest q in quest.Requesites.QuestsToGive)
                GameInstance.GameState.QuestController.Add(q);
        }
    }

    [System.Serializable]
    sealed public class QuestSavable : Savable
    {
        private List<QuestInfo> questsInfo;

        public List<QuestInfo> QuestsInfo { get { return questsInfo; } }

        public QuestSavable(List<QuestID> quests)
            : base ("", "")
        {
            questsInfo = new List<QuestInfo>();
            foreach (QuestID id in quests)
                questsInfo.Add(new QuestInfo(id));
        }
    }

    public delegate void EventHandler(QuestController sender);
    public delegate void QuestEventHandler(QuestController sender, QuestID quest);

    public event QuestEventHandler  OnQuestAdded,
                                    OnQuestRemoved,
                                    OnQuestCompleted;

    [SerializeField]
    private List<QuestID> quests = new List<QuestID>(); 

    public List<QuestID> Quests { get { return quests; } }

    public PlayerController PlayerController { get; private set; }

    public void Initialize (PlayerController controller)
    {
        PlayerController = controller;
    }

    public void Update ()
    {
        if (PlayerController == null)
            return;

        foreach (QuestID id in quests.ToArray())
            if (!id.completed && id.quest.Requesites.IsValid(PlayerController))
                id.Complete();
    }

    public void Clear ()
    {
        quests.Clear();
    }

    public void Add(QuestID id)
    {
        if (Get(id.quest.name) != null)
            return;

        quests.Add(id);

        id.OnCompleted += OnQuestHasBeenCompleted;
    }

    public void Add(Quest quest)
    {
        if (Get(quest.name) != null)
            return;

        QuestID auxQuest = new QuestID(quest);

        quests.Add(auxQuest);

        auxQuest.OnCompleted += OnQuestHasBeenCompleted;

        OnQuestAdded?.Invoke(this, auxQuest);
    }

    private void OnQuestHasBeenCompleted(QuestID sender)
    {
        OnQuestCompleted?.Invoke(this, sender);
    }

    public void Remove(Quest quest)
    {
        foreach (QuestID auxQuest in quests)
            if (auxQuest.quest.name.Equals(quest.name))
            {
                quests.Remove(auxQuest);
                OnQuestRemoved?.Invoke(this, auxQuest);
                break;
            }
    }

    public QuestID Get(string name)
    {
        return quests.Find(x => x.quest.name.Equals(name));
    }

    public void CompleteQuest(string name)
    {
        string auxName = name.ToLower();

        QuestID id = quests.Find(x => x.quest.name.ToLower().Equals(auxName));
        if (id != null)
            id.Complete();
    }

    Savable ISavable.IO { get { return new QuestSavable(quests); } }
}
