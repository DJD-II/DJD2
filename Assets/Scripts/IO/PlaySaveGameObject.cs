using System.Collections.Generic;

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
            io2 = objects.Find(x => io.ID.Equals(x.ID));
        else
            io2 = objects.Find(x => io.ID.Equals(x.ID) && io.SceneName.Equals(x.SceneName));

        if (io2 != null)
            objects.Remove(io2);

        objects.Add(io);
    }

    public Savable Get(string id, string sceneName, bool persistent)
    {
        if (persistent)
            return objects.Find(i => id == i.ID);

        return objects.Find(i => id == i.ID && i.SceneName == sceneName);
    }
}
