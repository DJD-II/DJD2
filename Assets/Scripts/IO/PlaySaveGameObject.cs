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


    public void Feed(ISavable savable, bool persistent)
    {
        Savable io = savable.IO,
                io2;

        if (persistent)
            io2 = objects.Find(x => io.id.Equals(x.id));
        else
            io2 = objects.Find(x => io.id.Equals(x.id) && io.sceneName.Equals(x.sceneName));

        if (io2 != null)
            objects.Remove(io2);

        objects.Add(io);
    }

    public Savable Get(string id, string sceneName, bool persistent)
    {
        if (persistent)
            return objects.Find(i => id == i.id);

        return objects.Find(i => id == i.id && i.sceneName == sceneName);
    }
}
