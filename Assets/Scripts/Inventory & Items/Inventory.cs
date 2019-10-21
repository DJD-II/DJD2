using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This represents the inventory of a character 
/// or object in the world.
/// </summary>
[System.Serializable]
sealed public class Inventory
{
    /// <summary>
    /// List of items.
    /// </summary>
    [SerializeField]
    private List<Item> items = new List<Item>();

    /// <summary>
    /// List of the inventory items.
    /// </summary>
    public List<Item> Items { get { return items; } }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(Item item)
    {
        Items.Add(item);
    }

    /// <summary>
    /// Removed an Item from the inventory.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Remove (string name)
    {
        Item i = items.Find(x => x.name.ToLower().Equals(name.ToLower()));
        if (i == null)
            return false;

        return items.Remove(i);
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
    public int GetAmmount (string name)
    {
        return items.Where(x=> x.Name.ToLower().Equals(name.ToLower())).Count();
    }

    /// <summary>
    /// Checks if an item exists in the inventory.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name)
    {
        return Items.Find(x => x.name.ToLower().Equals(name.ToLower())) != null;
    }
}