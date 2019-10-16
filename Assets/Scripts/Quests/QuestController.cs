using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
sealed public class QuestController : object
{
    sealed public class QuestID
    {
        public delegate void EventHandler(QuestID sender);

        public event EventHandler OnCompleted;

        public Quest quest;
        public bool completed;

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
        }
    }

    public delegate void EventHandler(QuestController sender);
    public delegate void QuestEventHandler(QuestController sender, QuestID quest);

    public event QuestEventHandler  OnQuestAdded,
                                    OnQuestRemoved,
                                    OnQuestCompleted;

    [SerializeField]
    private List<QuestID> quests = new List<QuestID>();

    public List<QuestID> Quest { get { return quests; } }

    public void Add (Quest quest)
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
                break;
            }
    }

    public QuestID Get (string name)
    {
        return quests.Find(x => x.quest.name.Equals(name));
    }

    public void CompleteQuest (string name)
    {
        string auxName = name.ToLower();

        QuestID id = quests.Find(x => x.quest.name.ToLower().Equals(auxName));
        if (id != null)
            id.Complete();
    }
}
