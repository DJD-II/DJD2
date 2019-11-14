[System.Serializable]
sealed public class QuestID : object
{
    public delegate void EventHandler(QuestID sender);

    public event EventHandler OnCompleted;

    public Quest quest;
    public bool completed;

    public QuestID(QuestInfo info)
    {
        quest = QuestUtility.Get(info.Name);
        completed = info.Completed;
        OnCompleted = null;
    }

    public QuestID(Quest quest)
    {
        this.quest = quest;
        completed = false;
        OnCompleted = null;
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
