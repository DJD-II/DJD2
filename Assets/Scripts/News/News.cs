using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New News", menuName = "News")]
sealed public class News : ScriptableObject
{
    public enum Layout : byte
    {
        Layout1 = 0,
        Layout2 = 1,
        Layout3 = 2,
    }

    public Layout layout;
    public Sprite icon;
    public string headline;
    public string text1;
    public string text2;
}