using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemUtility
{
    private readonly static Item[] items = Resources.LoadAll<Item>("Item Types");
    public static Item GetItem(string name)
    {
        foreach (Item item in items)
            if (name.Equals(item.tag.ToString()))
                return item;

        return null;
    }
}
