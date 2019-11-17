[System.Serializable]
public struct QuestInfo
{
    public string Name { get; }
    public bool Completed { get; }

    public QuestInfo(QuestID id)
    {
        Name = id.quest.name;
        Completed = id.completed;
    }
}
