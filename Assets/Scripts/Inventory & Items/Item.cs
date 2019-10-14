using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum Tag
    {
        Battery,
        Used_Battery,
        Oil,
        Bobby_Pin,
        Bubble_Gum,
        Scrap_Metal,
    }

    public Sprite icon;
    public Tag tag;
    public string description;
    public float weight;
    public float cost;

    public void Use(PlayerController controller)
    {
        switch (tag)
        {
            case Tag.Battery:
                controller.Hp.Add(50);
                Debug.Log("Added HP to " + controller.gameObject.name);
                break;
            case Tag.Used_Battery:
                controller.Hp.Add(20);
                break;
            case Tag.Oil:

                break;
            case Tag.Bubble_Gum:

                break;
        }
    }

    public string Name { get { return tag.ToString().Replace("_", " "); } }
}

[System.Serializable]
public class ItemID
{
    public string name;

    public ItemID(Item i)
    {
        name = i.tag.ToString();
    }
}
