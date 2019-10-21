using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
sealed public class Quest : ScriptableObject
{
    public new string name;
    public string description;
    public bool hide;
    [SerializeField]
    private QuestRequesite requesites = null;
    public Vector3[] position;

    public QuestRequesite Requesites { get { return requesites; } }
}
