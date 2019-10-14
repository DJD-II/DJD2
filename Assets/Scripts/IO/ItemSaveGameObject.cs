using System.Collections.Generic;

[System.Serializable]
public abstract class Savable : object
{
    public string sceneName;
    public string id;

    public Savable(string id, string sceneName)
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
public class PlaySaveGameObject : IO.Object
{
    public List<Savable> objects;

    public PlaySaveGameObject()
    {
        objects = new List<Savable>();
    }
}
