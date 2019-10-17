using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
sealed public class Quest : ScriptableObject, IOperatable<bool>
{
    public new string name;
    public string description;
    public bool hide;
    [SerializeField]
    private QuestRequesite requesites = null;
    public Vector3[] position;

    public QuestRequesite Requesites { get { return requesites; } }

    bool IOperatable<bool>.Get(LogicOperator operation, bool value)
    {
        QuestController.QuestID id = GameInstance.GameState.QuestController.Get(name);
        if (id == null)
            return false;

        switch (operation)
        {
            case LogicOperator.Equal:
                return value == id.completed;

            default:
                return false;
        }
    }
}
