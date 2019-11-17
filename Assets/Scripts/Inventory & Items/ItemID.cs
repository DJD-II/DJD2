[System.Serializable]
public class ItemID
{
    public string Name { get; private set; }

    public ItemID(Item i)
    {
        Name = i.ItemTag.ToString();
    }
}
