using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
sealed public class Quest : ScriptableObject
{
    public new string name;
    public string description;
    public bool hide;
    public Vector3[] position;
}
