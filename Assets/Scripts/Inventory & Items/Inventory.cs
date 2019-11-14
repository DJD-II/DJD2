using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This represents the inventory of a character 
/// or object in the world.
/// </summary>
[System.Serializable]
sealed public class Inventory : ILogicOperatable<Item>, IEnumerable<Item>
{
    /// <summary>
    /// List of items.
    /// </summary>
    [SerializeField]
    private List<Item> items = new List<Item>();

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(Item item)
    {
        items.Add(item);
    }

    /// <summary>
    /// Adds a collection of items to the inventory.
    /// </summary>
    /// <param name="itemsToEarn">The items to be added.</param>
    public void Add (IEnumerable<Item> itemsToEarn)
    {
        foreach (Item i in itemsToEarn)
            Add(i);
    }

    /// <summary>
    /// Removed an Item from the inventory.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Remove(string name)
    {
        Item i = items.Find(x => x.Name.ToLower().Equals(name.ToLower()));
        if (i == null)
            return false;

        return items.Remove(i);
    }

    public bool Remove(Item item)
    {
        return Remove(item.Name);
    }

    /// <summary>
    /// Removes a collection of items from the inventory.
    /// </summary>
    /// <param name="itemsToGive">The items to be removed.</param>
    public void Remove(IEnumerable<Item> itemsToGive)
    {
        foreach (Item i in itemsToGive)
            Remove(i.Name);
    }

    /// <summary>
    /// Clears all items from the inventory.
    /// </summary>
    public void Clear()
    {
        items.Clear();
    }

    /// <summary>
    /// Gets the ammount of an item in the inventory.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetAmmount(string name)
    {
        return items.Where(x => x.Name.ToLower().Equals(name.ToLower())).Count();
    }

    /// <summary>
    /// Checks if an item exists in the inventory.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name)
    {
        return items.Find(x => x.Name.ToLower().Equals(name.ToLower())) != null;
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    bool ILogicOperatable<Item>.Get(Item value)
    {
        return Contains(value.Name);
    }
}