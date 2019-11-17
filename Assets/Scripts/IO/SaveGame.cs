using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is an IO object that holds various 
/// savable (serializable) objects to be 
/// dumped to the (save) file.
/// Savable objects, are objects that hold information about 
/// other (ISavable) objects. These savable objects
/// only store useful info (from ISavable objects).
/// </summary>
[System.Serializable]
sealed public class SaveGame : IO.Object
{
    /// <summary>
    /// A list that contains all savable objects to be saved.
    /// Later on this object will be dumped to disk with all 
    /// these objects attached.
    /// </summary>
    private List<Savable> Objects { get; } = new List<Savable>();

    /// <summary>
    /// Finds a savable object whithin the list of savable objects.
    /// </summary>
    /// <param name="match">The predicate match to find in the list.</param>
    /// <returns>A savable object.</returns>
    public Savable Find(Predicate<Savable> match)
    {
        return Objects.Find(match);
    }

    /// <summary>
    /// Adds a savable object to the list.
    /// </summary>
    /// <param name="savable">The savable object to be saved.</param>
    private void Add(Savable savable)
    {
        Objects.Add(savable);
    }

    /// <summary>
    /// Removes a savable object from the list.
    /// </summary>
    /// <param name="savable">The savable object to be removed.</param>
    /// <returns></returns>
    private bool Remove(Savable savable)
    {
        return Objects.Remove(savable);
    }

    /// <summary>
    /// Adds or overrites a new savable object to the list.
    /// </summary>
    /// <param name="savable">The savable object to save.</param>
    /// <param name="id">The Object's id.</param>
    /// <param name="persistent">Whether the object is persistent through scenes.</param>
    public void Override(ISavable savable, string id = "", bool persistent = false)
    {
        Savable saveObject = savable.IO,
                saveObject2;

        // If the id is empty or its null then we'll treat this savable
        // as a unique savable object whithin the savable object list. 
        // Therefore the object will be unique in the list.
        if (string.IsNullOrEmpty(id))
        {
            Savable auxSave = Find(x => x.GetType().Equals(saveObject.GetType()));
            // Remove any existing Savable object with the same type.
            if (auxSave != null)
                Remove(auxSave);
            // Add the new savable object.
            Add(saveObject);

            return;
        }

        // If the savable object is persistent through scenes then 
        // We'll only search objects in the list through an id.
        // Otherwise We'll search objects that have the same 
        // id and that are in the same scene. This means that even if
        // two objects have the same id the algorithm checks if the scene is
        // the same. If not, then treat the two objects as different.
        if (persistent)
            saveObject2 = Find(x => saveObject.ID.Equals(x.ID));
        else
            saveObject2 = Find(x => saveObject.ID.Equals(x.ID) && saveObject.SceneName.Equals(x.SceneName));

        // Remove old savable object.
        // If we are adding a new one with the same 
        // id and/or with the same scene name.
        if (saveObject2 != null)
            Remove(saveObject2);

        // Add the new savable object.
        Add(saveObject);
    }

    /// <summary>
    /// Gets a savable object in the list, from an id.
    /// </summary>
    /// <param name="id">The savable object id.</param>
    /// <param name="persistent">If the savable is persistent through scenes.</param>
    /// <returns>A savable object.</returns>
    public Savable Get(string id, bool persistent)
    {
        if (persistent)
            return Find(i => id == i.ID);

        return Find(i => id == i.ID && i.SceneName == SceneManager.GetActiveScene().name);
    }
}
