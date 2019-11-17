using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed public class QuestController : ISavable
{
    [System.Serializable]
    sealed public class QuestSavable : Savable
    {
        public List<QuestInfo> QuestsInfo { get; }

        public QuestSavable(List<QuestID> quests)
            : base("", "")
        {
            QuestsInfo = new List<QuestInfo>();
            foreach (QuestID id in quests)
                QuestsInfo.Add(new QuestInfo(id));
        }

        public void Set(QuestController controller)
        {
            foreach (QuestInfo info in QuestsInfo)
                controller.Add(new QuestID(info));
        }
    }

    public delegate void EventHandler(QuestController sender);
    public delegate void QuestEventHandler(QuestController sender, QuestID quest);

    public event QuestEventHandler OnQuestAdded,
                                    OnQuestRemoved,
                                    OnQuestCompleted;

    [SerializeField]
    private List<QuestID> quests = new List<QuestID>();

    public List<QuestID> Quests { get { return quests; } }

    public PlayerController PlayerController { get; private set; }

    public void Initialize(PlayerController controller)
    {
        PlayerController = controller;
    }

    public void Update()
    {
        if (PlayerController == null)
            return;

        foreach (QuestID id in quests.ToArray())
            if (!id.completed && id.quest.Requesites.Fullfills(PlayerController))
                id.Complete();
    }

    private void Clear()
    {
        quests.Clear();
    }

    private void Add(QuestID id)
    {
        if (Get(id.quest.name) != null)
        {
            Debug.LogWarning("Quest already registred");
            return;
        }

        quests.Add(id);

        OnQuestAdded?.Invoke(this, id);

        id.OnCompleted += OnQuestHasBeenCompleted;
    }

    public void Add(Quest quest)
    {
        Add(new QuestID(quest));
    }

    public void Add (IEnumerable<Quest> questsToEarn)
    {
        foreach (Quest q in questsToEarn)
            Add(q);
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

    public void CompleteQuests(List<Quest> questsToComplete)
    {
        foreach (Quest q in questsToComplete)
        {
            QuestID id = quests.Find(
                x => x.quest.name.ToLower().Equals(q.name.ToLower()));

            if (id != null)
                id.Complete();
        }
    }

    public void Load(SaveGame io)
    {
        Clear();
        if (io.Find(x => x is QuestSavable) is QuestSavable savable)
            savable.Set(this);
    }

    Savable ISavable.IO { get { return new QuestSavable(quests); } }
}
