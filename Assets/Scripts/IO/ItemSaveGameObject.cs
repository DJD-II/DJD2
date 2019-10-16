using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Savable : object
{
    public string sceneName;
    public string id;

    public Savable (string id, string sceneName)
    {
        this.id = id;
        this.sceneName = sceneName;
    }
}

public interface ISavable
{
    Savable IO { get; }
}

[System.Serializable]
public class PlaySaveGameObject : SaveGame.Object
{
    public List<Savable> objects;

    public PlaySaveGameObject()
    {
        objects = new List<Savable>();
    }
}
