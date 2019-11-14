using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
sealed public class Quest : ScriptableObject, ILogicOperatable<bool>
{
    public new string name;
    public string description;
    public bool hide;
    [SerializeField]
    private QuestRequesite requesites = null;
    public Vector3[] position;

    public QuestRequesite Requesites { get { return requesites; } }

    bool ILogicOperatable<bool>.Get(bool value)
    {
        QuestID id = GameInstance.GameState.QuestController.Get(name);
        if (id == null)
            return false;

        return value == id.completed;
    }
}
